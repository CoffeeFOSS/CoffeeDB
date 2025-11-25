using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities.Revision;
using Backend.Enums;

namespace Backend.Interfaces.Repository;

public interface IRevisionMetadataRepository
{
  /// <summary>
  /// Retrieves a RevisionMetadata.
  /// </summary>
  /// <param name="id">The ID of the RevisionMetadata to retrieve.</param>
  /// <returns><see cref="RevisionMetadataDto"/> if found; otherwise, <c>null</c>.</returns>
  Task<RevisionMetadataDto?> GetRevisionMetadataAsync(int id);

  /// <summary>
  /// Retrieves a list of RevisionMetadata authored by user with UserId
  /// </summary>
  /// <param name="userRevisionParams">Query params for revisions.</param>
  /// <param name="userId">The ID of the User accessing this endpoint.</param>
  /// <param name="statuses">A list of revision statuses to filter by.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataWithEntityIdentifierDto"/>.</returns>
  Task<PagedList<RevisionMetadataWithEntityIdentifierDto>> GetUserRevisionMetadatasAsync(UserRevisionParams userRevisionParams, int userId, List<RevisionStatus> statuses);

  /// <summary>
  /// Retrieves a list of pending RevisionMetadata. If the user is not a moderator, only their own authored pending revisions would be retrieved.
  /// </summary>
  /// <param name="revisionParams">Query params for revisions.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataWithEntityIdentifierDto"/>.</returns>
  Task<PagedList<RevisionMetadataWithEntityIdentifierDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams);

  /// <summary>
  /// Retrieves a list of committed RevisionMetadata authored by a user.
  /// </summary>
  /// <param name="revisionParams">Query params for revisions.</param>
  /// <param name="userId">The ID of the user to find committed revision metadata of.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataDto"/>.</returns>
  Task<PagedList<RevisionMetadataWithEntityIdentifierDto>> GetCommittedRevisionMetadatasAsync(RevisionParams revisionParams, int userId);

  /// <summary>
  /// Change status of a RevisionMetadata from Pending to Rejected. 
  /// </summary>
  /// <param name="id">The ID of the RevisionMetadata to reject.</param>
  /// <param name="rejecterUserId">The ID of the User who rejected the revision.</param>
  /// <returns><c>true</c> if the rejection was successful; otherwise, <c>false</c>.</returns>
  Task<bool> RejectPendingRevisionMetadataAsync(int id, int rejecterUserId);

  /// <summary>
  /// Changes status of a RevisionMetadata from Pending to Committed. 
  /// </summary>
  /// <param name="id">The ID of the RevisionMetadata to reject.</param>
  /// <param name="approverUserId">The ID of the User who approved the revision.</param>
  /// <param name="latestVersion">The Version number of the current/latest revision for the Entity.</param>
  /// <returns><c>true</c> if the approval was successful; otherwise, <c>false</c>.</returns>
  Task<bool> ApprovePendingRevisionMetadataAsync(int id, int approverUserId, int? latestVersion = 0);

  /// <summary>
  /// Updates all children revisions with a matching ParentRevisionId to a new parent. 
  /// </summary>
  /// <param name="oldParentRevisionId">The ID of the revision to abandon the children from.</param>
  /// <param name="newParentRevisionId">The ID of the revision to adopt the orphaned children.</param>
  /// <param name="approverUserId">The ID of the User who approved the revision.</param>
  /// <returns>The number of adopted revision metadatas.</returns>
  Task<int> AdoptPendingRevisionMetadatasAsync(int oldParentRevisionId, int newParentRevisionId, int approverUserId);

  /// <summary>
  /// Creates a RevisionMetadata. (Careful: A RevisionMetadata must have a corresponding EntityRevision with the same primary key ID!) 
  /// </summary>
  /// <param name="comment">A short description of the reason this revision is necessary.</param>
  /// <param name="userId">The ID of the User who created the revision.</param>
  /// <param name="parentRevisionId">The ID of the revision metadata which was used as a base to create this revision.</param>
  /// <returns><see cref="RevisionMetadata"/></returns>
  RevisionMetadata CreateRevisionMetadataAsync(string comment, int userId, int? parentRevisionId);

  /// <summary>
  /// Creates a RevisionMetadata. (Careful: A RevisionMetadata must have a corresponding EntityRevision with the same primary key ID!) 
  /// </summary>
  /// <param name="revisionId">The ID of the revision to update.</param>
  /// <param name="comment">A short description of the reason this revision is necessary.</param>
  /// <param name="userId">The ID of the User who updated the revision.</param>
  /// <returns><see cref="RevisionMetadata"/> if found; otherwise, <c>null</c>.</returns>
  Task<RevisionMetadata?> UpdateRevisionMetadataAsync(int revisionId, string comment, int userId);
}
