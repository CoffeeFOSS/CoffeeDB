using Backend.Common;
using Backend.DTOs;
using Backend.Entities;

namespace Backend.Interfaces;

public interface IRoasterRepository
{
  /// <summary>
  /// Retrieves a list of roasters.
  /// </summary>
  /// <param name="userParams">Demanded pagination data.</param>
  /// <returns>A paginated list of <see cref="RoasterDto"/> objects.</returns>
  Task<PagedList<RoasterDto>> GetRoastersAsync(UserParams userParams);

  /// <summary>
  /// Retrieves a roaster by its unique ID.
  /// </summary>
  /// <param name="id">The ID of the roaster to retrieve.</param>
  /// <returns><see cref="Roaster"/> if found; otherwise, <c>null</c>.</returns>
  Task<RoasterDto?> GetRoasterByIdAsync(int id);

  Task<PagedList<RoasterDto>> GetFilteredRoastersAsync();
  Task<RoasterDto?> CreateRoasterAsync();
  Task<RoasterDto?> UpdateRoasterAsync();
  Task<bool> DeleteRoasterAsync();
}
