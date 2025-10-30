using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities;

namespace Backend.Interfaces.Repository;

public interface IRoasterRepository
{
  /// <summary>
  /// Retrieves a list of roasters.
  /// </summary>
  /// <param name="roasterParams">Query params for roasters.</param>
  /// <returns>A paginated list of <see cref="RoasterDto"/> objects.</returns>
  Task<PagedList<RoasterDto>> GetRoastersAsync(RoasterParams roasterParams);

  /// <summary>
  /// Retrieves a roaster by its unique ID.
  /// </summary>
  /// <param name="id">The ID of the roaster to retrieve.</param>
  /// <returns><see cref="Roaster"/> if found; otherwise, <c>null</c>.</returns>
  Task<RoasterDto?> GetRoasterByIdAsync(int id);

  Task<RoasterDto?> CreateRoasterAsync(CreateRoasterDto createRoasterDto);
  Task<RoasterDto?> UpdateRoasterAsync(int id, UpdateRoasterDto updateRoasterDto);
  Task<bool> DeleteRoasterAsync();
  Task<bool> RoasterExistsAsync(string name, string? location, int? excludeId = null);
  Task<bool> RoasterExistsByIdAsync(int id);
}
