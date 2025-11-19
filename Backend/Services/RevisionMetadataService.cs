using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Enums;
using Backend.Extensions;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class RevisionMetadataService(IRevisionMetadataRepository revisionMetadataRepository) : IRevisionMetadataService
{
  public async Task<ServiceResult<RevisionMetadataDto>> GetRevisionMetadataAsync(int id)
  {
    var revision = await revisionMetadataRepository.GetRevisionMetadataAsync(id);
    if (revision == null)
      return ServiceResult<RevisionMetadataDto>.Failure(404, $"Revision ID '{id}' not found.");

    return ServiceResult<RevisionMetadataDto>.Success(200, revision);
  }

  public async Task<PagedList<RevisionMetadataDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams, HttpResponse response)
  {
    var revisions = await revisionMetadataRepository.GetPendingRevisionMetadatasAsync(revisionParams);
    response.AddPaginationHeader(revisions);

    return revisions;
  }

  public async Task<ServiceResult<object>> RejectRevisionMetadataAsync(int id, ClaimsPrincipal userClaims)
  {
    var revisionMetadata = await revisionMetadataRepository.GetRevisionMetadataAsync(id);
    if (revisionMetadata == null)
      return ServiceResult<object>.Failure(400, $"Entity Revision ID '{id}' not found");

    if (revisionMetadata.Status != RevisionStatus.Pending.ToString())
      return ServiceResult<object>.Failure(403, $"Cannot reject Entity Revision ID '{id}' because its status is not 'Pending'.");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var result = await revisionMetadataRepository.RejectPendingRevisionMetadataAsync(id, userId);
    if (!result)
      return ServiceResult<object>.Failure(500, $"Could not reject entity revision ID '{id}'");

    return ServiceResult<object>.Success(200, null);
  }
}
