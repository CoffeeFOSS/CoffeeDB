using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Enums;
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

  public async Task<ServiceResult<object>> RejectEntityRevisionAsync(int id, ClaimsPrincipal userClaims)
  {
    var entityRevision = await revisionRepository.GetEntityRevisionAsync(id);
    if (entityRevision == null)
      return ServiceResult<object>.Failure(400, $"Entity Revision ID '{id}' not found");

    if (entityRevision.Status != RevisionStatus.Pending.ToString())
      return ServiceResult<object>.Failure(403, $"Cannot reject Entity Revision ID '{id}' because its status is not 'Pending'.");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var result = await revisionRepository.RejectPendingEntityRevisionAsync(id, userId);

    if (!result)
      return ServiceResult<object>.Failure(500, $"Could not reject entity revision ID '{id}'");

    return ServiceResult<object>.Success(200, null);
  }
}
