using Backend.Common.Params;
using Backend.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class RevisionsController(IRevisionMetadataService RevisionMetadataService) : BaseApiController
{
	[Authorize(Policy = "RequireModeratorRole")]
	[HttpGet("{revisionId:int}")]
	[ProducesResponseType(200)]
	[ProducesResponseType(401)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetRevisionMetadata(int revisionId)
		=> (await RevisionMetadataService.GetRevisionMetadataAsync(revisionId)).ToActionResult();

	[Authorize]
	[HttpGet("/api/users/{userId:int}/revisions")]
	[ProducesResponseType(200)]
	[ProducesResponseType(401)]
	public async Task<IActionResult> GetUserRevisionMetadatas([FromQuery] UserRevisionParams userRevisionParams, int userId)
		=> (await RevisionMetadataService.GetUserRevisionMetadatasAsync(userId, userRevisionParams, Response, User)).ToActionResult();

	[Authorize(Policy = "RequireModeratorRole")]
	[HttpGet("pending")]
	[ProducesResponseType(200)]
	[ProducesResponseType(401)]
	[ProducesResponseType(403)]
	// TODO: RevisionParams is ambiguous, make it less ambiguous by making separate Param classes
	public async Task<IActionResult> GetPendingRevisionMetadatas([FromQuery] RevisionParams revisionParams)
		=> Ok(await RevisionMetadataService.GetPendingRevisionMetadatasAsync(revisionParams, Response));

	[Authorize]
	[HttpGet("committed/{userId:int}")]
	[ProducesResponseType(200)]
	[ProducesResponseType(401)]
	public async Task<IActionResult> GetCommittedRevisionMetadatas([FromQuery] RevisionParams revisionParams, int userId)
		=> Ok(await RevisionMetadataService.GetCommittedRevisionMetadatasAsync(revisionParams, Response, userId));

	[Authorize(Policy = "RequireModeratorRole")]
	[HttpPost("{id:int}/reject")]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	[ProducesResponseType(403)]
	[ProducesResponseType(500)]
	public async Task<IActionResult> RejectRevisionMetadata(int id)
		=> (await RevisionMetadataService.RejectRevisionMetadataAsync(id, User)).ToActionResult();
}
