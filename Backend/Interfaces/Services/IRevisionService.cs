using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IRevisionService
{
  Task<ServiceResult<RevisionMetadataDto>> GetRevisionMetadataAsync(int id);
  Task<PagedList<RevisionMetadataDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams, HttpResponse response);
  Task<ServiceResult<object>> RejectRevisionMetadataAsync(int id, ClaimsPrincipal userClaims);
}
