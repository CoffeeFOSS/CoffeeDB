using Backend.Common;
using Backend.DTOs;

namespace Backend.Interfaces;

public interface IRoasterService
{
  /// <summary>
  /// Gets all roasters.
  /// </summary>
  /// <param name="userParams">Pagination settings from user.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of roaster information.</returns>
  Task<PagedList<RoasterDto>> GetRoastersAsync(UserParams userParams, HttpResponse response);

  /// <summary>
  /// Gets a roaster by their ID.
  /// </summary>
  /// <param name="id">The ID of the roaster.</param>
  /// <returns>The roaster information.</returns>
  Task<ServiceResult<RoasterDto>> GetRoasterAsync(int id);
}