using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Enums;
using Backend.Extensions;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class RoasterService(IRoasterRepository roasterRepository, IRevisionMetadataRepository revisionMetadataRepository) : IRoasterService
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

    var roasterRevisionSnapshot = await roasterRepository.CreateInitialRoasterRevisionAsync(revisionMetadata, createRoasterRevisionDto);
    if (roasterRevisionSnapshot == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(500, "Could not create initial roaster revision");

    return ServiceResult<RoasterRevisionSnapshotDto>.Success(200, roasterRevisionSnapshot);
  }

  public async Task<ServiceResult<RoasterRevisionSnapshotDto>> CreateRoasterRevisionAsync(int id, CreateRoasterRevisionDto createRoasterRevisionDto, ClaimsPrincipal userClaims)
  {
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

    var roasterRevisionSnapshot = await roasterRepository.CreateRoasterRevisionAsync(id, revisionMetadata, createRoasterRevisionDto);
    if (roasterRevisionSnapshot == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(500, $"Could not create roaster revision for Roaster ID '{id}'");

    return ServiceResult<RoasterRevisionSnapshotDto>.Success(200, roasterRevisionSnapshot);
  }

  public async Task<ServiceResult<object>> DeleteRoasterAsync(int id)
  {
    if (!await roasterRepository.RoasterExistsByIdAsync(id))
      return ServiceResult<object>.Failure(400, $"Roaster ID '{id}' does not exist");

    var result = await roasterRepository.DeleteRoasterAsync(id);

    if (!result)
      return ServiceResult<object>.Failure(500, "Could not delete roaster (other entities reference roaster)");

    return ServiceResult<object>.Success(200, null);
  }

  public async Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetRoasterRevisionExcerptsAsync(
    RevisionParams revisionExcerptParams, int roasterId, HttpResponse response, ClaimsPrincipal user)
  {
    var roaster = await roasterRepository.GetRoasterByIdAsync(roasterId);

    if (roaster == null)
      return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Failure(400, $"Roaster ID '{roasterId}' does not exist");

    var ignoreStatus = user?.IsInRole("Moderator") == true;
    var roasterRevisions = await roasterRepository.GetRoasterRevisionExcerptsAsync(revisionExcerptParams, roasterId, ignoreStatus);
    response.AddPaginationHeader(roasterRevisions);

    return ServiceResult<PagedList<RevisionMetadataExcerptDto>>.Success(200, roasterRevisions);
  }

  public async Task<ServiceResult<RoasterRevisionSnapshotDto>> GetRoasterRevisionSnapshotAsync(int revisionId, ClaimsPrincipal user)
  {
    var ignoreStatus = user?.IsInRole("Moderator") == true;
    var roasterRevision = await roasterRepository.GetRoasterRevisionSnapshotAsync(revisionId, ignoreStatus);

    if (roasterRevision == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(404, $"Roaster Revision ID '{revisionId}' not found or has not been committed");

    return ServiceResult<RoasterRevisionSnapshotDto>.Success(200, roasterRevision);
  }

  public async Task<ServiceResult<RoasterRevisionDiffDto>> GetRoasterRevisionDiffAsync(int revisionId1, int revisionId2, int roasterId, ClaimsPrincipal user)
  {
    if (revisionId1 == revisionId2)
      return ServiceResult<RoasterRevisionDiffDto>.Failure(400, $"Roaster Revision ID '{revisionId1}' cannot be compared with itself");

    var ignoreStatus = user?.IsInRole("Moderator") == true;

    var roasterRevision1 = await roasterRepository.GetRoasterRevisionSnapshotAsync(revisionId1, ignoreStatus);
    if (roasterRevision1 == null)
      return ServiceResult<RoasterRevisionDiffDto>.Failure(400, $"Roaster Revision ID '{revisionId1}' not found or has not been committed");

    var roasterRevision2 = await roasterRepository.GetRoasterRevisionSnapshotAsync(revisionId2, ignoreStatus);
    if (roasterRevision2 == null)
      return ServiceResult<RoasterRevisionDiffDto>.Failure(400, $"Roaster Revision ID '{revisionId2}' not found or has not been committed");

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
      return ServiceResult<RoasterDto>.Failure(403, $"Cannot reject Entity Revision ID '{revisionId}' because its status is not 'Pending'.");

    var roasterRevisionEntity = await roasterRepository.GetRoasterRevisionEntityAsync(revisionId);
    if (roasterRevisionEntity == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster Revision for the Entity Revision ID '{revisionId}' does not exist, is the entity type wrong?");

    if (roasterRevisionEntity.RoasterId != null)
      return ServiceResult<RoasterDto>.Failure(400, $"Cannot create Roaster because Roaster Revision with ID '{revisionId}' is tied to an existing Roaster with ID '{roasterRevisionEntity.RoasterId}'.");

    var parentRevisionId = revisionMetadata.ParentRevisionId;
    if (parentRevisionId != null)
      return ServiceResult<RoasterDto>.Failure(400, $"Entity Revision cannot have a Parent Revision ID when creating a new Roaster. Current Parent Revision ID: {parentRevisionId}");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    // Updates to the DB

    var roasterDto = await roasterRepository.CreateRoasterAsync(roasterRevisionEntity);
    if (roasterDto == null)
      return ServiceResult<RoasterDto>.Failure(500, $"Could not create Roaster for roaster revision ID {roasterRevisionEntity.Id}");

    var approvalResult = await revisionMetadataRepository.ApprovePendingRevisionMetadataAsync(roasterRevisionEntity.Id, userId);
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
      return ServiceResult<RoasterDto>.Failure(403, $"Cannot reject Entity Revision ID '{revisionId}' because its status is not 'Pending'.");

    var roasterRevisionEntity = await roasterRepository.GetRoasterRevisionEntityAsync(revisionId);
    if (roasterRevisionEntity == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster Revision for the Entity Revision ID '{revisionId}' does not exist, is the entity type wrong?");

    if (roasterRevisionEntity.RoasterId != roasterId)
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster ID {roasterRevisionEntity.RoasterId} on Roaster Revision and provided Roaster ID {roasterId} are different.");

    var roaster = await roasterRepository.GetRoasterByIdAsync(roasterId);
    if (roaster == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster of ID {roasterId} does not exist.");

    var oldParentRevisionId = revisionMetadata.ParentRevisionId;
    if (oldParentRevisionId == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Entity Revision must have a Parent Revision ID when updating an existing Roaster.");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    // Updates to DB

    var roasterDto = await roasterRepository.UpdateRoasterAsync(roasterRevisionEntity);
    if (roasterDto == null)
      return ServiceResult<RoasterDto>.Failure(500, $"Could not update Roaster with ID {roasterId} for roaster revision with ID {roasterRevisionEntity.Id}");

    var approvalResult = await revisionMetadataRepository.ApprovePendingRevisionMetadataAsync(revisionId, userId);
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
