using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities.Revision;

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
  /// <returns><see cref="RoasterDto"/> if found; otherwise, <c>null</c>.</returns>
  Task<RoasterDto?> GetRoasterByIdAsync(int id);

  /// <summary>
  /// Creates a roaster.
  /// </summary>
  /// <param name="roasterRevision">The revision snapshot of the roaster.</param>
  /// <returns><see cref="RoasterDto"/> if successfully created; otherwise, <c>null</c>.</returns>
  Task<RoasterDto?> CreateRoasterAsync(RoasterRevision roasterRevision);

  /// <summary>
  /// Updates a roaster's details.
  /// </summary>
  /// <param name="roasterRevision">The revision snapshot of the roaster.</param>
  /// <returns><see cref="RoasterDto"/> if successfully updated; otherwise, <c>null</c>.</returns>
  Task<RoasterDto?> UpdateRoasterAsync(RoasterRevision roasterRevision);

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

  Task<PagedList<RoasterRevisionExcerptDto>> GetRoasterRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int roasterId, bool ignoreStatus);
  Task<RoasterRevisionSnapshotDto?> GetRoasterRevisionSnapshotAsync(int revisionId, bool ignoreStatus);
  Task<RoasterRevision?> GetRoasterRevisionEntityAsync(int revisionId);

  Task<RevisionMetadataVersioningDto?> GetLatestRoasterRevisionVersionAsync(int roasterId);
  Task<RoasterRevisionSnapshotDto?> CreateInitialRoasterRevisionAsync(RevisionMetadataDto revisionMetadata, CreateRoasterRevisionDto createRoasterRevisionDto);
  Task<RoasterRevisionSnapshotDto?> CreateRoasterRevisionAsync(int roasterId, RevisionMetadataDto revisionMetadata, CreateRoasterRevisionDto updateRoasterDto);
}
