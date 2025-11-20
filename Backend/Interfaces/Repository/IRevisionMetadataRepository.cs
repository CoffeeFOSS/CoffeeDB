using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
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
  /// Retrieves a list of pending RevisionMetadata. If the user is not a moderator, only their own authored pending revisions would be retrieved.
  /// </summary>
  /// <param name="revisionParams">Query params for revisions.</param>
  /// <param name="userIsModerator">Will enable search of every RevisionMetadata tied to the Roaster regardless of Status if true.</param>
  /// <param name="userId">The ID of the User accessing this endpoint.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataDto"/>.</returns>
  Task<PagedList<RevisionMetadataDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams, bool userIsModerator, int? userId);

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
  /// <returns><c>true</c> if child revisions do not exist or child revisions were changed; otherwise, <c>false</c>.</returns>
  Task<bool> AdoptPendingRevisionMetadatasAsync(int oldParentRevisionId, int newParentRevisionId, int approverUserId);

  /// <summary>
  /// Creates a RevisionMetadata. (Careful: A RevisionMetadata must have a corresponding EntityRevision with the same primary key ID!) 
  /// </summary>
  /// <param name="comment">A short description of the reason this revision is necessary.</param>
  /// <param name="userId">The ID of the User who created the revision.</param>
  /// <param name="entityType">The enum of the Entity Type.</param>
  /// <param name="parentRevisionId">The ID of the revision metadata which was used as a base to create this revision.</param>
  /// <returns></returns>
  Task<RevisionMetadataDto?> CreateRevisionMetadataAsync(string comment, int userId, EntityType entityType, int? parentRevisionId);
}
