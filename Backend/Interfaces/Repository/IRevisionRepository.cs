using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Repository;

public interface IRevisionRepository
{
  Task<RevisionMetadataDto?> GetRevisionMetadataAsync(int id);
  Task<PagedList<RevisionMetadataDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams);
  Task<bool> RejectPendingRevisionMetadataAsync(int id, int rejecterUserId);
  Task<bool> ApprovePendingRevisionMetadataAsync(int id, int approverUserId);
  Task<bool> AdoptPendingRevisionMetadatasAsync(int oldParentRevisionId, int newParentRevisionId, int approverUserId);
}
