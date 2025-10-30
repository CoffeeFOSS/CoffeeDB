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

  // ADD DTO
  Task<ServiceResult<RoasterDto>> CreateRoasterAsync();
  Task<ServiceResult<RoasterDto>> UpdateRoasterAsync();
  Task<ServiceResult<bool>> DeleteRoasterAsync(); // admin only
}