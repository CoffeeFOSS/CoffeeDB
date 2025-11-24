using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Enums;
using Backend.Extensions;
using Backend.Interfaces;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class RoasterService(IUnitOfWork unitOfWork, IRoasterRepository roasterRepository, IRevisionMetadataRepository revisionMetadataRepository) : IRoasterService
{
  public async Task<PagedList<RoasterDto>> GetRoastersAsync(RoasterParams roasterParams, HttpResponse response)
  {
    var roasters = await roasterRepository.GetRoastersAsync(roasterParams);
    response.AddPaginationHeader(roasters);

    return roasters;
  }

  public async Task<ServiceResult<RoasterDto>> GetRoasterAsync(int id)
  {
    var roaster = await roasterRepository.GetRoasterByIdAsync(id);

    if (roaster == null)
      return ServiceResult<RoasterDto>.Failure(404, $"Roaster with ID {id} not found");

    return ServiceResult<RoasterDto>.Success(200, roaster);
  }

  public async Task<ServiceResult<RoasterRevisionSnapshotDto>> CreateInitialRoasterRevisionAsync(CreateRoasterRevisionDto createRoasterRevisionDto, ClaimsPrincipal userClaims)
  {
    if (
      (createRoasterRevisionDto.LocationCoordinateLatitude.HasValue && !createRoasterRevisionDto.LocationCoordinateLongitude.HasValue) ||
      (!createRoasterRevisionDto.LocationCoordinateLatitude.HasValue && createRoasterRevisionDto.LocationCoordinateLongitude.HasValue))
    {
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, "If either one of latitude or longitude are provided, both must be provided.");
    }

    if (string.IsNullOrWhiteSpace(createRoasterRevisionDto.Name))
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, "Name must be provided");

    if (await roasterRepository.RoasterExistsAsync(createRoasterRevisionDto.Name, createRoasterRevisionDto.LocationAddress))
    {
      if (createRoasterRevisionDto.LocationAddress == null)
        return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"Roaster '{createRoasterRevisionDto.Name}' already exists without a specified location address");
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"Roaster '{createRoasterRevisionDto.Name}' already exists at address '{createRoasterRevisionDto.LocationAddress}'");
    }

    if (!string.IsNullOrWhiteSpace(createRoasterRevisionDto.WebsiteUrl) && !UrlValidator.IsValidUrl(createRoasterRevisionDto.WebsiteUrl))
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"'{createRoasterRevisionDto.WebsiteUrl}' is not a valid URL'");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = await revisionMetadataRepository.CreateRevisionMetadataAsync(createRoasterRevisionDto.Comment, userId, EntityType.Roaster, null);
    if (revisionMetadata == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(500, "Unable to create Entity Revision Metadata for initial roaster.");

    var roasterRevision = await roasterRepository.CreateInitialRoasterRevisionAsync(revisionMetadata, createRoasterRevisionDto);

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(500, "Could not create initial roaster revision");

    var roasterRevisionSnapshotDto = new RoasterRevisionSnapshotDto
    {
      // CreatedBy, UpdatedBy will both be null at this point because EF hasn't pulled revisionMetadata data from DB
      Id = roasterRevision.Id,
      RoasterId = roasterRevision.RoasterId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      Name = roasterRevision.Name,
      Alias = roasterRevision.Alias,
      LocationAddress = roasterRevision.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roasterRevision.LocationCoordinates),
      WebsiteUrl = roasterRevision.WebsiteUrl,
      Description = roasterRevision.Description,
    };

    return ServiceResult<RoasterRevisionSnapshotDto>.Success(200, roasterRevisionSnapshotDto);
  }

  public async Task<ServiceResult<RoasterRevisionSnapshotDto>> CreateRoasterRevisionAsync(int id, CreateRoasterRevisionDto createRoasterRevisionDto, ClaimsPrincipal userClaims)
  {
    if (
      (createRoasterRevisionDto.LocationCoordinateLatitude.HasValue && !createRoasterRevisionDto.LocationCoordinateLongitude.HasValue) ||
      (!createRoasterRevisionDto.LocationCoordinateLatitude.HasValue && createRoasterRevisionDto.LocationCoordinateLongitude.HasValue))
    {
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, "If either one of latitude or longitude are provided, both must be provided.");
    }

    var currentRoaster = await roasterRepository.GetRoasterByIdAsync(id);

    if (currentRoaster == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"Roaster ID '{id}' does not exist");

    if (!string.IsNullOrWhiteSpace(createRoasterRevisionDto.Name))
    {
      if (await roasterRepository.RoasterExistsAsync(createRoasterRevisionDto.Name, createRoasterRevisionDto.LocationAddress, id))
      {
        string locationInfo = createRoasterRevisionDto.LocationAddress == null ?
            "without a specified location address" : $"at location address '{createRoasterRevisionDto.LocationAddress}'";
        return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400,
            $"Roaster '{createRoasterRevisionDto.Name}' already exists {locationInfo}.");
      }
    }

    if (!string.IsNullOrWhiteSpace(createRoasterRevisionDto.WebsiteUrl) && !UrlValidator.IsValidUrl(createRoasterRevisionDto.WebsiteUrl))
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"'{createRoasterRevisionDto.WebsiteUrl}' is not a valid URL'");

    // Should make a revision, not update roaster.
    // CreateRoasterRevisionAsync will be used when revision is approved and committed
    // Since we're making RoasterRevision AND RevisionEntity, unit of work would help here

    bool hasChanges =
      currentRoaster.Name != createRoasterRevisionDto.Name ||
      currentRoaster.Alias != createRoasterRevisionDto.Alias ||
      currentRoaster.LocationAddress != createRoasterRevisionDto.LocationAddress ||
      currentRoaster.WebsiteUrl != createRoasterRevisionDto.WebsiteUrl ||
      currentRoaster.Description != createRoasterRevisionDto.Description ||
      (createRoasterRevisionDto.LocationCoordinateLatitude.HasValue &&
      currentRoaster.LocationCoordinates?.Latitude != createRoasterRevisionDto.LocationCoordinateLatitude) ||
      (createRoasterRevisionDto.LocationCoordinateLongitude.HasValue &&
      currentRoaster.LocationCoordinates?.Longitude != createRoasterRevisionDto.LocationCoordinateLongitude);

    if (!hasChanges)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"No changes detected for Roaster ID '{id}'. Revision not created.");

    var currentRoasterRevisionVersioning = await roasterRepository.GetLatestRoasterRevisionVersionAsync(currentRoaster.Id);

    if (currentRoasterRevisionVersioning == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"Revision version for Roaster ID '{id}' does not exist");

    var currentRoasterId = currentRoasterRevisionVersioning.Id; // this will be parentRevisionId for the new revision

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var revisionMetadata = await revisionMetadataRepository.CreateRevisionMetadataAsync(createRoasterRevisionDto.Comment, userId, EntityType.Roaster, currentRoasterId);
    if (revisionMetadata == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(500, $"Unable to create Entity Revision Metadata for Roaster ID '{id}'.");

    var roasterRevision = await roasterRepository.CreateRoasterRevisionAsync(id, revisionMetadata, createRoasterRevisionDto);

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(500, $"Could not create roaster revision for Roaster ID '{id}'");

    var roasterRevisionSnapshotDto = new RoasterRevisionSnapshotDto
    {
      // CreatedBy, UpdatedBy will both be null at this point because EF hasn't pulled revisionMetadata data from DB
      Id = roasterRevision.Id,
      RoasterId = roasterRevision.RoasterId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      ParentRevisionId = revisionMetadata.ParentRevisionId,
      Name = roasterRevision.Name,
      Alias = roasterRevision.Alias,
      LocationAddress = roasterRevision.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roasterRevision.LocationCoordinates),
      WebsiteUrl = roasterRevision.WebsiteUrl,
      Description = roasterRevision.Description,
    };

    return ServiceResult<RoasterRevisionSnapshotDto>.Success(200, roasterRevisionSnapshotDto);
  }

  public async Task<ServiceResult<RoasterRevisionSnapshotDto>> UpdateRoasterRevisionAsync(int revisionId, CreateRoasterRevisionDto createRoasterRevisionDto, ClaimsPrincipal userClaims)
  {
    if (
      (createRoasterRevisionDto.LocationCoordinateLatitude.HasValue && !createRoasterRevisionDto.LocationCoordinateLongitude.HasValue) ||
      (!createRoasterRevisionDto.LocationCoordinateLatitude.HasValue && createRoasterRevisionDto.LocationCoordinateLongitude.HasValue))
    {
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, "If either one of latitude or longitude are provided, both must be provided.");
    }

    var userId = UserClaimsUtils.GetUserId(userClaims);
    var revisionSnapshot = await roasterRepository.GetRoasterRevisionSnapshotAsync(revisionId, false, userId);
    if (revisionSnapshot == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"Revision ID '{revisionId}' does not exist");

    if (revisionSnapshot.Status != RevisionStatus.Pending.ToString() && revisionSnapshot.Status != RevisionStatus.Draft.ToString())
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"Revision ID '{revisionId}' is not pending or draft");

    if (revisionSnapshot.CreatedById != userId)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(403, $"User ID {userId} is not the author of Revision ID '{revisionId}'");

    RoasterDto? currentRoaster = null;
    var roasterId = revisionSnapshot.RoasterId;
    if (roasterId != null)
    {
      currentRoaster = await roasterRepository.GetRoasterByIdAsync(roasterId.Value);
      if (currentRoaster == null)
        return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"Roaster ID '{roasterId}' does not exist");
    }

    if (!string.IsNullOrWhiteSpace(createRoasterRevisionDto.Name))
    {
      if (await roasterRepository.RoasterExistsAsync(createRoasterRevisionDto.Name, createRoasterRevisionDto.LocationAddress, roasterId))
      {
        string locationInfo = createRoasterRevisionDto.LocationAddress == null ?
            "without a specified location address" : $"at location address '{createRoasterRevisionDto.LocationAddress}'";
        return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400,
            $"Roaster '{createRoasterRevisionDto.Name}' already exists {locationInfo}.");
      }
    }

    if (!string.IsNullOrWhiteSpace(createRoasterRevisionDto.WebsiteUrl) && !UrlValidator.IsValidUrl(createRoasterRevisionDto.WebsiteUrl))
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"'{createRoasterRevisionDto.WebsiteUrl}' is not a valid URL'");

    // Should make a revision, not update roaster.
    // CreateRoasterRevisionAsync will be used when revision is approved and committed
    // Since we're making RoasterRevision AND RevisionEntity, unit of work would help here

    bool hasChangesCurrentRoasterToRevisionDto = currentRoaster == null ||
      currentRoaster.Name != createRoasterRevisionDto.Name ||
      currentRoaster.Alias != createRoasterRevisionDto.Alias ||
      currentRoaster.LocationAddress != createRoasterRevisionDto.LocationAddress ||
      currentRoaster.WebsiteUrl != createRoasterRevisionDto.WebsiteUrl ||
      currentRoaster.Description != createRoasterRevisionDto.Description ||
      (createRoasterRevisionDto.LocationCoordinateLatitude.HasValue &&
      currentRoaster.LocationCoordinates?.Latitude != createRoasterRevisionDto.LocationCoordinateLatitude) ||
      (createRoasterRevisionDto.LocationCoordinateLongitude.HasValue &&
      currentRoaster.LocationCoordinates?.Longitude != createRoasterRevisionDto.LocationCoordinateLongitude);

    if (!hasChangesCurrentRoasterToRevisionDto)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"No changes detected for Roaster ID '{roasterId}'. Revision not updated.");

    bool hasChangesCurrentRevisionToRevisionDto =
      revisionSnapshot.Name != createRoasterRevisionDto.Name ||
      revisionSnapshot.Alias != createRoasterRevisionDto.Alias ||
      revisionSnapshot.LocationAddress != createRoasterRevisionDto.LocationAddress ||
      revisionSnapshot.WebsiteUrl != createRoasterRevisionDto.WebsiteUrl ||
      revisionSnapshot.Description != createRoasterRevisionDto.Description ||
      (createRoasterRevisionDto.LocationCoordinateLatitude.HasValue &&
      revisionSnapshot.LocationCoordinates?.Latitude != createRoasterRevisionDto.LocationCoordinateLatitude) ||
      (createRoasterRevisionDto.LocationCoordinateLongitude.HasValue &&
      revisionSnapshot.LocationCoordinates?.Longitude != createRoasterRevisionDto.LocationCoordinateLongitude);

    if (!hasChangesCurrentRevisionToRevisionDto)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"No changes detected for Revision ID '{revisionId}'. Revision not updated.");

    var roasterRevision = await roasterRepository.UpdateRoasterRevisionAsync(revisionId, createRoasterRevisionDto);
    if (roasterRevision == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"Roaster Revision ID '{revisionId}' does not exist.");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(500, $"Could not update Roaster Revision '{revisionId}'.");

    var roasterRevisionSnapshotDto = new RoasterRevisionSnapshotDto
    {
      Id = roasterRevision.Id,
      RoasterId = roasterRevision.RoasterId,
      Comment = roasterRevision.RevisionMetadata.Comment,
      Version = roasterRevision.RevisionMetadata.Version,
      Status = roasterRevision.RevisionMetadata.Status.ToString(),
      ParentRevisionId = roasterRevision.RevisionMetadata.ParentRevisionId,
      Name = roasterRevision.Name,
      Alias = roasterRevision.Alias,
      LocationAddress = roasterRevision.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roasterRevision.LocationCoordinates),
      WebsiteUrl = roasterRevision.WebsiteUrl,
      Description = roasterRevision.Description,
      CreatedAt = roasterRevision.RevisionMetadata.CreatedAt,
      CreatedBy = roasterRevision.RevisionMetadata.CreatedBy?.UserName,
      CreatedById = roasterRevision.RevisionMetadata.CreatedBy?.Id,
      UpdatedAt = roasterRevision.RevisionMetadata.UpdatedAt,
      UpdatedBy = roasterRevision.RevisionMetadata.UpdatedBy?.UserName,
      UpdatedById = roasterRevision.RevisionMetadata.UpdatedBy?.Id,
    };

    var revisionMetadata = await revisionMetadataRepository.UpdateRevisionMetadataAsync(revisionId, createRoasterRevisionDto.Comment, userId, EntityType.Roaster);
    if (revisionMetadata == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(500, $"Could not update Entity Revision Metadata for Roaster ID '{revisionId}'.");

    return ServiceResult<RoasterRevisionSnapshotDto>.Success(200, roasterRevisionSnapshotDto);
  }

  public async Task<ServiceResult<object>> DeleteRoasterAsync(int id)
  {
    var roasterExists = await roasterRepository.DeleteRoasterAsync(id);
    if (!roasterExists)
      return ServiceResult<object>.Failure(400, $"Roaster ID '{id}' does not exist");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<object>.Failure(500, "Could not delete roaster (other entities reference roaster)");

    return ServiceResult<object>.Success(200, null);
  }

  public async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetRoasterRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, int roasterId, HttpResponse response, ClaimsPrincipal user)
  {
    var roaster = await roasterRepository.GetRoasterByIdAsync(roasterId);

    if (roaster == null)
      return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Failure(400, $"Roaster ID '{roasterId}' does not exist");

    var userIsModerator = user?.IsInRole("Moderator") == true;
    var roasterRevisions = await roasterRepository.GetRoasterRevisionExcerptsAsync(revisionExcerptParams, roasterId, userIsModerator);
    response.AddPaginationHeader(roasterRevisions);
    response.Headers.Append("Roaster-Name", roaster.Name);
    response.Headers.AccessControlExposeHeaders = "Pagination, Roaster-Name";

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, roasterRevisions);
  }

  public async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetNewRoasterRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, HttpResponse response)
  {
    var roasterRevisions = await roasterRepository.GetNewRoasterRevisionExcerptsAsync(revisionExcerptParams);
    response.AddPaginationHeader(roasterRevisions);

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, roasterRevisions);
  }

  public async Task<ServiceResult<RoasterRevisionSnapshotDto>> GetRoasterRevisionSnapshotAsync(int revisionId, ClaimsPrincipal user)
  {
    var userIsModerator = user.IsInRole("Moderator") == true;
    int? userId = UserClaimsUtils.GetUserIdOrNull(user);

    var roasterRevision = await roasterRepository.GetRoasterRevisionSnapshotAsync(revisionId, userIsModerator, userId);

    if (roasterRevision == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(404, $"Roaster Revision ID '{revisionId}' not found or user has no access");

    return ServiceResult<RoasterRevisionSnapshotDto>.Success(200, roasterRevision);
  }

  public async Task<ServiceResult<RoasterRevisionDiffDto>> GetRoasterRevisionDiffAsync(int revisionId1, int revisionId2, int roasterId, ClaimsPrincipal user)
  {
    if (revisionId1 == revisionId2)
      return ServiceResult<RoasterRevisionDiffDto>.Failure(400, $"Roaster Revision ID '{revisionId1}' cannot be compared with itself");

    var userIsModerator = user.IsInRole("Moderator") == true;
    int? userId = UserClaimsUtils.GetUserIdOrNull(user);

    var roasterRevision1 = await roasterRepository.GetRoasterRevisionSnapshotAsync(revisionId1, userIsModerator, userId);
    if (roasterRevision1 == null)
      return ServiceResult<RoasterRevisionDiffDto>.Failure(400, $"Roaster Revision ID '{revisionId1}' not found or user has no access.");

    var roasterRevision2 = await roasterRepository.GetRoasterRevisionSnapshotAsync(revisionId2, userIsModerator, userId);
    if (roasterRevision2 == null)
      return ServiceResult<RoasterRevisionDiffDto>.Failure(400, $"Roaster Revision ID '{revisionId2}' not found or user has no access.");

    // must check if the revisions are actually for the same roaster
    if (roasterRevision1.RoasterId != roasterRevision2.RoasterId)
      return ServiceResult<RoasterRevisionDiffDto>.Failure(400, $"Roaster Revision IDs {roasterRevision1.Id} and {roasterRevision2.Id} belong to different roasters");

    bool rev1Older = roasterRevision1.CreatedAt <= roasterRevision2.CreatedAt;
    var diff = CalculateDiff(rev1Older ? roasterRevision1 : roasterRevision2, rev1Older ? roasterRevision2 : roasterRevision1);

    return ServiceResult<RoasterRevisionDiffDto>.Success(200, diff);
  }

  private static RoasterRevisionDiffDto CalculateDiff(RoasterRevisionSnapshotDto oldRevision, RoasterRevisionSnapshotDto newRevision)
  {
    // if the roaster is initial version, then it wont have an ID.
    // but since roaster is initial version, there wont be two revisions of it to compare to.
    // we should never be able to get to this point if theres only an initial roaster revision,
    // so I'm suppressing the nullable warning 
    var diff = new RoasterRevisionDiffDto { RoasterId = oldRevision.RoasterId!.Value };

    // Add basic properties
    diff.Changes["id"] = new Change { Old = oldRevision.Id, New = newRevision.Id };
    diff.Changes["createdAt"] = new Change { Old = oldRevision.CreatedAt, New = newRevision.CreatedAt };
    diff.Changes["createdBy"] = new Change { Old = oldRevision.CreatedBy, New = newRevision.CreatedBy };
    diff.Changes["version"] = new Change { Old = oldRevision.Version, New = newRevision.Version };
    diff.Changes["comment"] = new Change { Old = oldRevision.Comment, New = newRevision.Comment };

    // Compare each property and only add if different
    if (oldRevision.Name != newRevision.Name)
      diff.Changes["name"] = new Change { Old = oldRevision.Name, New = newRevision.Name };

    if (oldRevision.Alias != newRevision.Alias)
      diff.Changes["alias"] = new Change { Old = oldRevision.Alias, New = newRevision.Alias };

    if (oldRevision.LocationAddress != newRevision.LocationAddress)
      diff.Changes["locationAddress"] = new Change { Old = oldRevision.LocationAddress, New = newRevision.LocationAddress };

    if (oldRevision.LocationCoordinates?.Latitude != newRevision.LocationCoordinates?.Latitude ||
        oldRevision.LocationCoordinates?.Longitude != newRevision.LocationCoordinates?.Longitude)
    {
      diff.Changes["locationCoordinates"] = new Change
      {
        Old = oldRevision.LocationCoordinates is not null
              ? new { oldRevision.LocationCoordinates.Latitude, oldRevision.LocationCoordinates.Longitude }
              : null,
        New = newRevision.LocationCoordinates is not null
              ? new { newRevision.LocationCoordinates.Latitude, newRevision.LocationCoordinates.Longitude }
              : null
      };
    }

    if (oldRevision.WebsiteUrl != newRevision.WebsiteUrl)
      diff.Changes["websiteUrl"] = new Change { Old = oldRevision.WebsiteUrl, New = newRevision.WebsiteUrl };

    if (oldRevision.Description != newRevision.Description)
      diff.Changes["description"] = new Change { Old = oldRevision.Description, New = newRevision.Description };

    return diff;
  }

  // the reason approve needs to be here is because if we only have revisionId, we dont know which DB table to query
  // is the revision in RoasterRevisions? BrewerRevisions? Idk! 
  // I can get the RevisionMetadata, but that tells me nothing about the EntityType (Roaster, Grinder, etc)

  public async Task<ServiceResult<RoasterDto>> ApproveCreateRoasterRevisionAsync(int revisionId, ClaimsPrincipal userClaims)
  {
    var revisionMetadata = await revisionMetadataRepository.GetRevisionMetadataAsync(revisionId);
    if (revisionMetadata == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Entity Revision ID '{revisionId}' not found");

    if (revisionMetadata.Status != RevisionStatus.Pending.ToString())
      return ServiceResult<RoasterDto>.Failure(403, $"Cannot approve Entity Revision ID '{revisionId}' because its status is not 'Pending'.");

    var parentRevisionId = revisionMetadata.ParentRevisionId;
    if (parentRevisionId != null)
      return ServiceResult<RoasterDto>.Failure(400, $"Entity Revision cannot have a Parent Revision ID when creating a new Roaster. Current Parent Revision ID: {parentRevisionId}");

    var roasterRevisionEntity = await roasterRepository.GetRoasterRevisionEntityAsync(revisionId);
    if (roasterRevisionEntity == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster Revision for the Entity Revision ID '{revisionId}' does not exist, is the entity type wrong?");

    if (roasterRevisionEntity.RoasterId != null)
      return ServiceResult<RoasterDto>.Failure(400, $"Cannot create Roaster because Roaster Revision with ID '{revisionId}' is tied to an existing Roaster with ID '{roasterRevisionEntity.RoasterId}'.");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    // Updates to the DB

    var roaster = await roasterRepository.CreateRoasterAsync(roasterRevisionEntity);

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<RoasterDto>.Failure(500, $"Could not create Roaster for roaster revision ID {roasterRevisionEntity.Id}");

    var roasterDto = new RoasterDto
    {
      Id = roaster.Id,
      Name = roaster.Name,
      Alias = roaster.Alias,
      LocationAddress = roaster.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roaster.LocationCoordinates),
      WebsiteUrl = roaster.WebsiteUrl,
      Description = roaster.Description,
    };

    var approvalResult = await revisionMetadataRepository.ApprovePendingRevisionMetadataAsync(roasterRevisionEntity.Id, userId, null);
    if (!approvalResult)
      return ServiceResult<RoasterDto>.Failure(500, $"Could not update roasterRevisionEntity {roasterRevisionEntity.Id} to committed status");

    // TODO: ideally later on once we add unit of work, the save all changes happen at the same time.

    return ServiceResult<RoasterDto>.Success(200, roasterDto);
  }

  public async Task<ServiceResult<RoasterDto>> ApproveUpdateRoasterRevisionAsync(int roasterId, int revisionId, ClaimsPrincipal userClaims)
  {
    var revisionMetadata = await revisionMetadataRepository.GetRevisionMetadataAsync(revisionId);
    if (revisionMetadata == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Entity Revision ID '{revisionId}' not found");

    if (revisionMetadata.Status != RevisionStatus.Pending.ToString())
      return ServiceResult<RoasterDto>.Failure(403, $"Cannot approve Entity Revision ID '{revisionId}' because its status is not 'Pending'.");

    var oldParentRevisionId = revisionMetadata.ParentRevisionId;
    if (oldParentRevisionId == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Entity Revision must have a Parent Revision ID when updating an existing Roaster.");

    var roasterRevisionEntity = await roasterRepository.GetRoasterRevisionEntityAsync(revisionId);
    if (roasterRevisionEntity == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster Revision for the Entity Revision ID '{revisionId}' does not exist, is the entity type wrong?");

    if (roasterRevisionEntity.RoasterId != roasterId)
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster ID {(roasterRevisionEntity.RoasterId == null ? "null" : roasterRevisionEntity.RoasterId)} on Roaster Revision and provided Roaster ID {roasterId} are different.");

    var currentRoasterRevisionVersioning = await roasterRepository.GetLatestRoasterRevisionVersionAsync(roasterId);
    if (currentRoasterRevisionVersioning == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Revision version for Roaster ID '{roasterId}' does not exist");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    // Updates to DB

    var roaster = await roasterRepository.UpdateRoasterAsync(roasterRevisionEntity);

    if (roaster == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster with ID {roasterId} does not exist");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<RoasterDto>.Failure(500, $"Could not update Roaster with ID {roasterId} for roaster revision with ID {roasterRevisionEntity.Id}");

    var roasterDto = new RoasterDto
    {
      Id = roaster.Id,
      Name = roaster.Name,
      Alias = roaster.Alias,
      LocationAddress = roaster.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roaster.LocationCoordinates),
      WebsiteUrl = roaster.WebsiteUrl,
      Description = roaster.Description,
    };

    var approvalResult = await revisionMetadataRepository.ApprovePendingRevisionMetadataAsync(revisionId, userId, currentRoasterRevisionVersioning.Version);
    if (!approvalResult)
      return ServiceResult<RoasterDto>.Failure(500, $"Could not update roasterRevisionEntity {roasterRevisionEntity.Id} to committed status");

    if (oldParentRevisionId != null)
    {
      var adoptionResult = await revisionMetadataRepository.AdoptPendingRevisionMetadatasAsync(oldParentRevisionId.Value, revisionId, userId);
      if (!adoptionResult)
        return ServiceResult<RoasterDto>.Failure(500, $"Could not adopt all children entities of roaster revision ID {oldParentRevisionId} to newly approved roaster revision ID {revisionId}");
    }

    // ideally later on once we add unit of work, the save all changes happen at the same time.

    return ServiceResult<RoasterDto>.Success(200, roasterDto);
  }
}
