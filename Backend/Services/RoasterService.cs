using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class RoasterService(IRoasterRepository roasterRepository) : IRoasterService
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

  public async Task<ServiceResult<RoasterDto>> CreateRoasterAsync(CreateRoasterDto createRoasterDto)
  {
    if (string.IsNullOrWhiteSpace(createRoasterDto.Name))
      return ServiceResult<RoasterDto>.Failure(400, "Name must be provided");

    if (await roasterRepository.RoasterExistsAsync(createRoasterDto.Name, createRoasterDto.LocationAddress))
    {
      if (createRoasterDto.LocationAddress == null)
        return ServiceResult<RoasterDto>.Failure(400, $"Roaster '{createRoasterDto.Name}' already exists without a specified location address");
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster '{createRoasterDto.Name}' already exists at address '{createRoasterDto.LocationAddress}'");
    }

    if (!string.IsNullOrWhiteSpace(createRoasterDto.WebsiteUrl) && !UrlValidator.IsValidUrl(createRoasterDto.WebsiteUrl))
      return ServiceResult<RoasterDto>.Failure(400, $"'{createRoasterDto.WebsiteUrl}' is not a valid URL'");

    var roaster = await roasterRepository.CreateRoasterAsync(createRoasterDto);

    if (roaster == null)
      return ServiceResult<RoasterDto>.Failure(500, "Could not create roaster");

    return ServiceResult<RoasterDto>.Success(200, roaster);
  }

  public async Task<ServiceResult<RoasterRevisionSnapshotDto>> UpdateRoasterAsync(int id, UpdateRoasterDto updateRoasterDto, ClaimsPrincipal userClaims)
  {
    var currentRoaster = await roasterRepository.GetRoasterByIdAsync(id);

    if (currentRoaster == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"Roaster ID '{id}' does not exist");

    if (!string.IsNullOrWhiteSpace(updateRoasterDto.Name))
    {
      if (await roasterRepository.RoasterExistsAsync(updateRoasterDto.Name, updateRoasterDto.LocationAddress, id))
      {
        string locationInfo = updateRoasterDto.LocationAddress == null ?
            "without a specified location address" : $"at location address '{updateRoasterDto.LocationAddress}'";
        return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400,
            $"Roaster '{updateRoasterDto.Name}' already exists {locationInfo}.");
      }
    }

    if (!string.IsNullOrWhiteSpace(updateRoasterDto.WebsiteUrl) && !UrlValidator.IsValidUrl(updateRoasterDto.WebsiteUrl))
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"'{updateRoasterDto.WebsiteUrl}' is not a valid URL'");

    // Should make a revision, not update roaster.
    // UpdateRoasterAsync will be used when revision is approved and committed
    // Since we're making RoasterRevision AND RevisionEntity, unit of work would help here

    bool hasChanges =
      currentRoaster.Name != updateRoasterDto.Name ||
      currentRoaster.Alias != updateRoasterDto.Alias ||
      currentRoaster.LocationAddress != updateRoasterDto.LocationAddress ||
      currentRoaster.WebsiteUrl != updateRoasterDto.WebsiteUrl ||
      currentRoaster.Description != updateRoasterDto.Description ||
      (updateRoasterDto.LocationCoordinateLatitude.HasValue &&
      currentRoaster.LocationCoordinates?.Latitude != updateRoasterDto.LocationCoordinateLatitude) ||
      (updateRoasterDto.LocationCoordinateLongitude.HasValue &&
      currentRoaster.LocationCoordinates?.Longitude != updateRoasterDto.LocationCoordinateLongitude);

    if (!hasChanges)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"No changes detected for Roaster ID '{id}'. Revision not created.");

    var currentRoasterRevisionVersioning = await roasterRepository.GetLatestRoasterRevisionVersionAsync(currentRoaster.Id);

    if (currentRoasterRevisionVersioning == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(400, $"Revision version for Roaster ID '{id}' does not exist");

    var currentRoasterId = currentRoasterRevisionVersioning.Id; // this will be parentRevisionId for the new revision

    var userIdString = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userIdString))
      throw new InvalidOperationException("User ID not found in claims");

    var userId = int.Parse(userIdString);

    var entityRevision = await roasterRepository.CreateEntityRevisionAsync(currentRoasterId, updateRoasterDto.Comment, userId);
    if (entityRevision == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(500, $"Unable to create Entity Revision Metadata for Roaster ID '{id}'.");

    var roasterRevisionSnapshot = await roasterRepository.CreateRoasterRevisionAsync(id, entityRevision, updateRoasterDto);
    if (roasterRevisionSnapshot == null)
      return ServiceResult<RoasterRevisionSnapshotDto>.Failure(500, "Could not update roaster (no changes in DTO or DB error)");

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

  public async Task<ServiceResult<PagedList<RoasterRevisionExcerptDto>>> GetRoasterRevisionExcerptsAsync(
    PaginationParams revisionExcerptParams, int roasterId, HttpResponse response, ClaimsPrincipal user)
  {
    var roaster = await roasterRepository.GetRoasterByIdAsync(roasterId);

    if (roaster == null)
      return ServiceResult<PagedList<RoasterRevisionExcerptDto>>.Failure(400, $"Roaster ID '{roasterId}' does not exist");

    var ignoreStatus = user?.IsInRole("Moderator") == true;
    var roasterRevisions = await roasterRepository.GetRoasterRevisionExcerptsAsync(revisionExcerptParams, roasterId, ignoreStatus);
    response.AddPaginationHeader(roasterRevisions);

    return ServiceResult<PagedList<RoasterRevisionExcerptDto>>.Success(200, roasterRevisions);
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
    diff.Changes["entityRevisionId"] = new Change { Old = oldRevision.EntityRevisionId, New = newRevision.EntityRevisionId };
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
}
