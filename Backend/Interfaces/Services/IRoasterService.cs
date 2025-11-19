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
  /// Creates a roaster revision for a roaster that doesn't exist yet.
  /// </summary>
  /// <param name="createRoasterRevisionDto">The details of the roaster.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created roaster revision snapshot.</returns>
  Task<ServiceResult<RoasterRevisionSnapshotDto>> CreateInitialRoasterRevisionAsync(CreateRoasterRevisionDto createRoasterRevisionDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Creates a roaster revision for a roaster that already exists.
  /// </summary>
  /// <param name="id">The ID of the roaster to update.</param>
  /// <param name="createRoasterRevisionDto">The updated details of the roaster.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>The created roaster revision snapshot.</returns>
  Task<ServiceResult<RoasterRevisionSnapshotDto>> CreateRoasterRevisionAsync(int id, CreateRoasterRevisionDto createRoasterRevisionDto, ClaimsPrincipal userClaims);

  /// <summary>
  /// Deletes a roaster from the database.
  /// </summary>
  /// <param name="id">The ID of the roaster to delete.</param>
  /// <returns>No content result.</returns>
  Task<ServiceResult<object>> DeleteRoasterAsync(int id);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for a Roaster. If the user is a moderator, will instead return excerpts regardless of status.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <param name="roasterId">The ID of the Roaster to find revision metadata for.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetRoasterRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int roasterId, HttpResponse response, ClaimsPrincipal userClaims);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for new Roasters.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetNewRoasterRevisionExcerptsAsync(RevisionParams revisionExcerptParams, HttpResponse response);

  /// <summary>
  /// Retrieves the Snapshot for the Roaster Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the Roaster Revision to retrieve the snapshot for.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="RoasterRevisionSnapshotDto"/></returns>
  Task<ServiceResult<RoasterRevisionSnapshotDto>> GetRoasterRevisionSnapshotAsync(int revisionId, ClaimsPrincipal userClaims);

  /// <summary>
  /// Retrieves a diff between two Roaster Revisions. The revisionId1 and revisionId2 order does not matter, the changes will always be compared from oldest to newest.
  /// </summary>
  /// <param name="revisionId1">The ID of a Roaster Revision to compare.</param>
  /// <param name="revisionId2">The ID of a Roaster Revision to compare, different from revisionId2.</param>
  /// <param name="roasterId">The ID of the Roaster both Revisions belong to.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="RoasterRevisionDiffDto"/></returns>
  Task<ServiceResult<RoasterRevisionDiffDto>> GetRoasterRevisionDiffAsync(int revisionId1, int revisionId2, int roasterId, ClaimsPrincipal userClaims);

  /// <summary>
  /// Approves a Roaster Revision that doesn't isn't tied to a Roaster ID, creating a new Roaster.
  /// </summary>
  /// <param name="revisionId">The ID of the Roaster Revision to create a Roaster with.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="RoasterDto"/></returns>
  Task<ServiceResult<RoasterDto>> ApproveCreateRoasterRevisionAsync(int revisionId, ClaimsPrincipal userClaims);

  /// <summary>
  /// Approves a Roaster Revision that is tied to a Roaster ID, updating the existing Roaster.
  /// </summary>
  /// <param name="roasterId">The ID of the Roaster to update.</param>
  /// <param name="revisionId">The ID of the Roaster Revision to update the Roaster with.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns><see cref="RoasterDto"/></returns>
  Task<ServiceResult<RoasterDto>> ApproveUpdateRoasterRevisionAsync(int roasterId, int revisionId, ClaimsPrincipal userClaims);
}