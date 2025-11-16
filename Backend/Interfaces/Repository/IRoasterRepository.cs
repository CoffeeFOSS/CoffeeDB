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

  /// <summary>
  /// Creates a roaster.
  /// </summary>
  /// <param name="createInitialRoasterRevisionDto">The details of the roaster.</param>
  /// <returns><see cref="Roaster"/> if successfully created; otherwise, <c>null</c>.</returns>
  Task<RoasterDto?> CreateInitialRoasterRevisionAsync(CreateInitialRoasterRevisionDto createInitialRoasterRevisionDto);

  /// <summary>
  /// Updates a roaster's details.
  /// </summary>
  /// <param name="id">The ID of the roaster to update.</param>
  /// <param name="updateRoasterDto">The updated details of the roaster.</param>
  /// <returns><see cref="Roaster"/> if successfully updated; otherwise, <c>null</c>.</returns>
  Task<RoasterDto?> CreateRoasterRevisionAsync(int id, CreateRoasterRevisionDto updateRoasterDto);

  /// <summary>
  /// Deletes a roaster from the database.
  /// </summary>
  /// <param name="id">The ID of the roaster to delete.</param>
  /// <returns><c>true</c> if successfully deleted; otherwise, <c>false</c>.</returns>
  Task<bool> DeleteRoasterAsync(int id);

  /// <summary>
  /// Checks if a roaster exists with a matching name and location address.
  /// </summary>
  /// <param name="name">The name of the roaster.</param>
  /// <param name="locationAddress">The location address of the roaster.</param>
  /// <param name="excludeId">The ID of the roaster to exclude from the check.</param>
  /// <returns><c>true</c> if roaster of matching name and location address found; otherwise, <c>false</c>.</returns>
  Task<bool> RoasterExistsAsync(string name, string? locationAddress, int? excludeId = null);

  /// <summary>
  /// Checks if a roaster with the ID exists.
  /// </summary>
  /// <param name="id">The roaster ID to check the existence of.</param>
  /// <returns><c>true</c> if a roaster with the ID exists; otherwise, <c>false</c>.</returns>
  Task<bool> RoasterExistsByIdAsync(int id);

  Task<PagedList<RoasterRevisionExcerptDto>> GetRoasterRevisionExcerptsAsync(PaginationParams revisionExcerptParams, int roasterId, bool ignoreStatus);
  Task<RoasterRevisionSnapshotDto?> GetRoasterRevisionSnapshotAsync(int revisionId, bool ignoreStatus);

  Task<RoasterRevisionVersioningDto?> GetLatestRoasterRevisionVersionAsync(int roasterId);
  Task<RoasterRevisionSnapshotDto?> CreateInitialRoasterRevisionAsync(EntityRevisionDto entityRevision, CreateInitialRoasterRevisionDto createInitialRoasterRevisionDto);
  Task<RoasterRevisionSnapshotDto?> CreateRoasterRevisionAsync(int roasterId, EntityRevisionDto entityRevision, CreateRoasterRevisionDto updateRoasterDto);
  Task<EntityRevisionDto?> CreateEntityRevisionAsync(string comment, int userId, int? parentRevisionId);
}
