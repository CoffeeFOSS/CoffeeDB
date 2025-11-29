using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Enums;
using Backend.Extensions;
using Backend.Interfaces;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class RevisionMetadataService(IUnitOfWork unitOfWork) : IRevisionMetadataService
{
  public async Task<ServiceResult<RevisionMetadataDto>> GetRevisionMetadataAsync(int id)
  {
    var revision = await unitOfWork.RevisionMetadataRepository.GetRevisionMetadataAsync(id);
    if (revision == null)
      return ServiceResult<RevisionMetadataDto>.Failure(404, $"Revision ID '{id}' not found.");

    return ServiceResult<RevisionMetadataDto>.Success(200, revision);
  }

  public async Task<ServiceResult<PagedList<RevisionMetadataWithEntityIdentifierDto>>> GetUserRevisionMetadatasAsync(int userId, UserRevisionParams userRevisionParams, HttpResponse response, ClaimsPrincipal userClaims)
  {
    var userIsModerator = userClaims.IsInRole("Moderator") == true;
    var authorizedUserId = UserClaimsUtils.GetUserId(userClaims);
    if (!userIsModerator && userId != authorizedUserId)
      return ServiceResult<PagedList<RevisionMetadataWithEntityIdentifierDto>>.Failure(403, $"User with ID {authorizedUserId} is not allowed to access non-committed revisions of user {userId}");

    List<RevisionStatus>? statuses = [];

    // check if status are valid
    if (!string.IsNullOrWhiteSpace(userRevisionParams.Status))
    {
      foreach (var status in userRevisionParams.Status.Split(',', StringSplitOptions.RemoveEmptyEntries))
      {
        if (!Enum.TryParse<RevisionStatus>(status, out RevisionStatus parsedVal))
          return ServiceResult<PagedList<RevisionMetadataWithEntityIdentifierDto>>.Failure(400, $"{status} is not a valid Revision Status");

        statuses.Add(parsedVal);
      }
    }

    var revisions = await unitOfWork.RevisionMetadataRepository.GetUserRevisionMetadatasAsync(userRevisionParams, authorizedUserId, statuses);
    response.AddPaginationHeader(revisions);

    return ServiceResult<PagedList<RevisionMetadataWithEntityIdentifierDto>>.Success(200, revisions);
  }

  public async Task<PagedList<RevisionMetadataWithEntityIdentifierDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams, HttpResponse response)
  {
    var revisions = await unitOfWork.RevisionMetadataRepository.GetPendingRevisionMetadatasAsync(revisionParams);
    response.AddPaginationHeader(revisions);

    return revisions;
  }

  public async Task<PagedList<RevisionMetadataWithEntityIdentifierDto>> GetCommittedRevisionMetadatasAsync(RevisionParams revisionParams, HttpResponse response, int userId)
  {
    var revisions = await unitOfWork.RevisionMetadataRepository.GetCommittedRevisionMetadatasAsync(revisionParams, userId);
    response.AddPaginationHeader(revisions);

    return revisions;
  }

  public async Task<ServiceResult<object>> RejectRevisionMetadataAsync(int id, ClaimsPrincipal userClaims)
  {
    var revisionMetadata = await unitOfWork.RevisionMetadataRepository.GetRevisionMetadataAsync(id);
    if (revisionMetadata == null)
      return ServiceResult<object>.Failure(404, $"Entity Revision ID '{id}' not found");

    if (revisionMetadata.Status != RevisionStatus.Pending.ToString())
      return ServiceResult<object>.Failure(403, $"Cannot reject Entity Revision ID '{id}' because its status is not 'Pending'.");

    var userId = UserClaimsUtils.GetUserId(userClaims);

    var result = await unitOfWork.RevisionMetadataRepository.RejectPendingRevisionMetadataAsync(id, userId);
    if (!result)
      return ServiceResult<object>.Failure(400, $"Entity Revision ID '{id}' does not exist");

    if (!await unitOfWork.SaveAllAsync())
      return ServiceResult<object>.Failure(500, $"Could not reject entity revision ID '{id}'");

    return ServiceResult<object>.Success(200, null);
  }
}
