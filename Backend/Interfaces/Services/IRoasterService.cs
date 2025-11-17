using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IRoasterService
{
  /// <summary>
  /// Gets all roasters.
  /// </summary>
  /// <param name="roasterParams">Pagination settings from user.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of roaster information.</returns>
  Task<PagedList<RoasterDto>> GetRoastersAsync(RoasterParams roasterParams, HttpResponse response);

  /// <summary>
  /// Gets a roaster by their ID.
  /// </summary>
  /// <param name="id">The ID of the roaster.</param>
  /// <returns>The roaster information.</returns>
  Task<ServiceResult<RoasterDto>> GetRoasterAsync(int id);

  /// <summary>
  /// Creates a roaster.
  /// </summary>
  /// <param name="createInitialRoasterRevisionDto">The details of the roaster.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created roaster information.</returns>
  Task<ServiceResult<RoasterRevisionSnapshotDto>> CreateInitialRoasterRevisionAsync(CreateInitialRoasterRevisionDto createInitialRoasterRevisionDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Updates a roaster's details.
  /// </summary>
  /// <param name="id">The ID of the roaster to update.</param>
  /// <param name="updateRoasterDto">The updated details of the roaster.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The newly created roaster revision snapshot.</returns>
  Task<ServiceResult<RoasterRevisionSnapshotDto>> CreateRoasterRevisionAsync(int id, CreateRoasterRevisionDto updateRoasterDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Deletes a roaster from the database.
  /// </summary>
  /// <param name="id">The ID of the roaster to delete.</param>
  /// <returns>No content result.</returns>
  Task<ServiceResult<object>> DeleteRoasterAsync(int id);

  Task<ServiceResult<PagedList<RoasterRevisionExcerptDto>>> GetRoasterRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int roasterId, HttpResponse response, ClaimsPrincipal user);
  Task<ServiceResult<RoasterRevisionSnapshotDto>> GetRoasterRevisionSnapshotAsync(int revisionId, ClaimsPrincipal userClaims);
  Task<ServiceResult<RoasterRevisionDiffDto>> GetRoasterRevisionDiffAsync(int revisionId1, int revisionId2, int roasterId, ClaimsPrincipal user);

  Task<ServiceResult<RoasterDto>> ApproveRoasterRevisionAsync(int roasterId, int revisionId);

  // oldParentRevision = the parent revision of the approved revision
  // newParentRevision = the approved revision
  // basically all the revisions that share the oldParentRevision will be rebased onto the approved revision
  // TODO (private?) RebaseRoasterRevisionChildren(int oldParentRevisionId, int newParentRevisionId)
}