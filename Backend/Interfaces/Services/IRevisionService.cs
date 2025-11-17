using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IRevisionService
{
  Task<ServiceResult<EntityRevisionDto>> GetEntityRevisionAsync(int id);
  Task<PagedList<EntityRevisionDto>> GetPendingEntityRevisionsAsync(RevisionParams revisionParams, HttpResponse response);
}
