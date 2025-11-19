using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IRevisionService
{
  /// <summary>
  /// Retrieves a RevisionMetadata.
  /// </summary>
  /// <param name="id">The ID of the RevisionMetadata to retrieve.</param>
  /// <returns><see cref="RevisionMetadataDto"/>.</returns>
  Task<ServiceResult<RevisionMetadataDto>> GetRevisionMetadataAsync(int id);

  /// <summary>
  /// Retrieves a list of pending RevisionMetadata.
  /// </summary>
  /// <param name="revisionParams">Query params for revisions.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataDto"/>.</returns>
  Task<PagedList<RevisionMetadataDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams, HttpResponse response);

  /// <summary>
  /// Change status of a RevisionMetadata from Pending to Rejected. 
  /// </summary>
  /// <param name="id">The ID of the RevisionMetadata to reject.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>No content.</returns>
  Task<ServiceResult<object>> RejectRevisionMetadataAsync(int id, ClaimsPrincipal userClaims);

  // Approvals for entities are done in their own respective services
}
