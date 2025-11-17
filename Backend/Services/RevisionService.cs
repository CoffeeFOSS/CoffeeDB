using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class RevisionService(IRevisionRepository revisionRepository) : IRevisionService
{
  public async Task<PagedList<EntityRevisionDto>> GetPendingEntityRevisionsAsync(RevisionParams revisionParams, HttpResponse response)
  {
    var revisions = await revisionRepository.GetPendingEntityRevisionsAsync(revisionParams);
    response.AddPaginationHeader(revisions);

    return revisions;
  }
}
