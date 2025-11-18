using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Repository;

public interface IRevisionRepository
{
  Task<EntityRevisionDto?> GetEntityRevisionAsync(int id);
  Task<PagedList<EntityRevisionDto>> GetPendingEntityRevisionsAsync(RevisionParams revisionParams);
  Task<bool> RejectPendingEntityRevisionAsync(int id, int rejecterUserId);
  Task<bool> ApprovePendingEntityRevisionAsync(int id, int approverUserId);
  Task<bool> AdoptPendingEntityRevisionsAsync(int oldParentRevisionId, int newParentRevisionId, int approverUserId);
  Task<bool> EntityRevisionExists(int id);
}
