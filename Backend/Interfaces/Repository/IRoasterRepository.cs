using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities;
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
  /// <returns><see cref="Roaster"/></returns>
  Task<Roaster> CreateRoasterAsync(RoasterRevision roasterRevision);

  /// <summary>
  /// Updates a roaster's details.
  /// </summary>
  /// <param name="roasterRevision">The revision snapshot of the roaster.</param>
  /// <returns><see cref="Roaster"/> if found; otherwise, <c>null</c>.</returns>
  Task<Roaster?> UpdateRoasterAsync(RoasterRevision roasterRevision);

  /// <summary>
  /// Deletes a roaster from the database.
  /// </summary>
  /// <param name="id">The ID of the roaster to delete.</param>
  /// <returns><c>true</c> if Roaster found; otherwise, <c>false</c>.</returns>
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
  /// <param name="id">The ID of the Roaster.</param>
  /// <returns><c>true</c> if a roaster with the ID exists; otherwise, <c>false</c>.</returns>
  Task<bool> RoasterExistsByIdAsync(int id);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for a Roaster.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <param name="roasterId">The ID of the Roaster.</param>
  /// <param name="userIsModerator">Will enable search of every RevisionMetadata tied to the Roaster regardless of Status if true.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<PagedList<RevisionMetadataExcerptDto>> GetRoasterRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int roasterId, bool userIsModerator);

  /// <summary>
  /// Retrieves a list of pending status RevisionMetadata excerpts for all new Roasters.
  /// </summary>
  /// <param name="revisionExcerptParams">Query params for revisions excerpts.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataExcerptDto"/></returns>
  Task<PagedList<RevisionMetadataExcerptDto>> GetNewRoasterRevisionExcerptsAsync(RevisionParams revisionExcerptParams);

  /// <summary>
  /// Retrieves the Snapshot for the Roaster Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the Roaster Revision.</param>
  /// <param name="userIsModerator">Will enable search of every RevisionMetadata tied to the Roaster regardless of Status if true.</param>
  /// <param name="userId">A possibly null ID of the user requesting the Snapshot.</param>
  /// <returns><see cref="RoasterRevisionSnapshotDto"/></returns>
  Task<RoasterRevisionSnapshotDto?> GetRoasterRevisionSnapshotAsync(int revisionId, bool userIsModerator, int? userId = null);

  /// <summary>
  /// Retrieves the database entity of the Roaster Revision. 
  /// </summary>
  /// <param name="revisionId">The ID of the Roaster Revision.</param>
  /// <returns><see cref="RoasterRevision"/></returns>
  Task<RoasterRevision?> GetRoasterRevisionEntityAsync(int revisionId);

  /// <summary>
  /// Retrieves the current (AKA latest version) committed Roaster Revision.
  /// </summary>
  /// <param name="roasterId">The ID of the Roaster.</param>
  /// <returns><see cref="RevisionMetadataVersioningDto"/></returns>
  Task<RevisionMetadataVersioningDto?> GetLatestRoasterRevisionVersionAsync(int roasterId);

  /// <summary>
  /// Creates a new Roaster Revision, that when approved, will create a new Roaster.
  /// </summary>
  /// <param name="revisionMetadata">The RevisionMetadata to tie this Roaster Revision to.</param>
  /// <param name="createRoasterRevisionDto">The details of the new Roaster.</param>
  /// <returns><see cref="RoasterRevision"/></returns>
  Task<RoasterRevision> CreateInitialRoasterRevisionAsync(RevisionMetadata revisionMetadata, CreateRoasterRevisionDto createRoasterRevisionDto);

  /// <summary>
  /// Creates a new Roaster Revision for an existing Roaster, that when approved, will update the existing Roaster.
  /// </summary>
  /// <param name="roasterId">The ID of the Roaster.</param>
  /// <param name="revisionMetadata">The RevisionMetadata to tie this Roaster Revision to.</param>
  /// <param name="createRoasterRevisionDto">The details of the updated Roaster.</param>
  /// <returns><see cref="RoasterRevision"/></returns>
  Task<RoasterRevision> CreateRoasterRevisionAsync(int roasterId, RevisionMetadata revisionMetadata, CreateRoasterRevisionDto createRoasterRevisionDto);

  /// <summary>
  /// Updates a Roaster Revision.
  /// </summary>
  /// <param name="revisionId">The ID of the Roaster Revision.</param>
  /// <param name="createRoasterRevisionDto">The details of the updated Roaster.</param>
  /// <returns><see cref="RoasterRevision"/> if found; otherwise, <c>null</c></returns>
  Task<RoasterRevision?> UpdateRoasterRevisionAsync(int revisionId, CreateRoasterRevisionDto createRoasterRevisionDto);
}
