using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class RevisionService(IRevisionRepository revisionRepository) : IRevisionService
{
  public async Task<ServiceResult<EntityRevisionDto>> GetEntityRevisionAsync(int id)
  {
    var revision = await revisionRepository.GetEntityRevisionAsync(id);
    if (revision == null)
      return ServiceResult<EntityRevisionDto>.Failure(404, $"Revision ID '{id}' not found.");

    return ServiceResult<EntityRevisionDto>.Success(200, revision);
  }

  public async Task<PagedList<EntityRevisionDto>> GetPendingEntityRevisionsAsync(RevisionParams revisionParams, HttpResponse response)
  {
    var revisions = await revisionRepository.GetPendingEntityRevisionsAsync(revisionParams);
    response.AddPaginationHeader(revisions);

    return revisions;
  }
}
