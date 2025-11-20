using Backend.Common.Params;
using Backend.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class RevisionsController(IRevisionMetadataService RevisionMetadataService) : BaseApiController
{
	[Authorize(Policy = "RequireModeratorRole")]
	[HttpGet("{id:int}")]
	[ProducesResponseType(200)]
	[ProducesResponseType(401)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetRevisionMetadata(int id)
		=> (await RevisionMetadataService.GetRevisionMetadataAsync(id)).ToActionResult();

	[Authorize]
	[HttpGet("pending")]
	[ProducesResponseType(200)]
	[ProducesResponseType(401)]
	public async Task<IActionResult> GetPendingRevisionMetadatas([FromQuery] RevisionParams revisionParams)
		=> Ok(await RevisionMetadataService.GetPendingRevisionMetadatasAsync(revisionParams, Response, User));

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
