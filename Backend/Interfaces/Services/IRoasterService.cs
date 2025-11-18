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

  Task<ServiceResult<PagedList<RevisionMetadataExcerptDto>>> GetRoasterRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int roasterId, HttpResponse response, ClaimsPrincipal userClaims);
  Task<ServiceResult<RoasterRevisionSnapshotDto>> GetRoasterRevisionSnapshotAsync(int revisionId, ClaimsPrincipal userClaims);
  Task<ServiceResult<RoasterRevisionDiffDto>> GetRoasterRevisionDiffAsync(int revisionId1, int revisionId2, int roasterId, ClaimsPrincipal userClaims);

  Task<ServiceResult<RoasterDto>> ApproveCreateRoasterRevisionAsync(int revisionId, ClaimsPrincipal userClaims);
  Task<ServiceResult<RoasterDto>> ApproveUpdateRoasterRevisionAsync(int roasterId, int revisionId, ClaimsPrincipal userClaims);
}