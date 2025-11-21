using System.Security.Claims;
using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IRevisionMetadataService
{
  /// <summary>
  /// Retrieves a RevisionMetadata.
  /// </summary>
  /// <param name="id">The ID of the RevisionMetadata to retrieve.</param>
  /// <returns><see cref="RevisionMetadataDto"/>.</returns>
  Task<ServiceResult<RevisionMetadataDto>> GetRevisionMetadataAsync(int id);

  /// <summary>
  /// Retrieves a list of pending RevisionMetadata. If the user is not a moderator, only their own authored pending revisions would be retrieved.
  /// </summary>
  /// <param name="revisionParams">Query params for revisions.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataWithEntityIdentifierDto"/>.</returns>
  Task<PagedList<RevisionMetadataWithEntityIdentifierDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams, HttpResponse response, ClaimsPrincipal userClaims);

  /// <summary>
  /// Retrieves a list of committed RevisionMetadata authored by a user.
  /// </summary>
  /// <param name="revisionParams">Query params for revisions.</param>
  /// <param name="response">HttpResponse object from controller.</param>
  /// <param name="userId">The ID of the user to find committed revision metadata of.</param>
  /// <returns>A paginated list of <see cref="RevisionMetadataDto"/>.</returns>
  Task<PagedList<RevisionMetadataWithEntityIdentifierDto>> GetCommittedRevisionMetadatasAsync(RevisionParams revisionParams, HttpResponse response, int userId);

  /// <summary>
  /// Change status of a RevisionMetadata from Pending to Rejected. 
  /// </summary>
  /// <param name="id">The ID of the RevisionMetadata to reject.</param>
  /// <param name="userClaims">Claims of the authenticated user.</param>
  /// <returns>No content.</returns>
  Task<ServiceResult<object>> RejectRevisionMetadataAsync(int id, ClaimsPrincipal userClaims);

  // Approvals for entities are done in their own respective services
}
