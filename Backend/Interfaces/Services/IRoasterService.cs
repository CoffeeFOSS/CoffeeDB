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
  /// <param name="createRoasterDto">The details of the roaster.</param>
  /// <returns>The created roaster information.</returns>
  Task<ServiceResult<RoasterDto>> CreateRoasterAsync(CreateRoasterDto createRoasterDto);

  /// <summary>
  /// Updates a roaster's details.
  /// </summary>
  /// <param name="id">The ID of the roaster to update.</param>
  /// <param name="updateRoasterDto">The updated details of the roaster.</param>
  /// <returns>The updated roaster information.</returns>
  Task<ServiceResult<RoasterDto>> UpdateRoasterAsync(int id, UpdateRoasterDto updateRoasterDto);

  /// <summary>
  /// Deletes a roaster from the database.
  /// </summary>
  /// <param name="id">The ID of the roaster to delete.</param>
  /// <returns>No content result.</returns>
  Task<ServiceResult<object>> DeleteRoasterAsync(int id);

  Task<ServiceResult<PagedList<RoasterRevisionExcerptDto>>> GetRoasterRevisionExcerptsAsync(PaginationParams revisionExcerptParams, int roasterId, HttpResponse response);
  Task<ServiceResult<RoasterRevisionSnapshotDto>> GetRoasterRevisionSnapshotAsync(int revisionId, int roasterId);
  Task<ServiceResult<RoasterRevisionDiffDto>> GetRoasterRevisionDiffAsync(int revisionId1, int revisionId2, int roasterId);
}