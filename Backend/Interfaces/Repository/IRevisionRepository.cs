using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Repository;

public interface IRevisionRepository
{
  Task<PagedList<EntityRevisionDto>> GetPendingEntityRevisionsAsync(RevisionParams revisionParams);
}
