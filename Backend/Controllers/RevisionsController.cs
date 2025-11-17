using Backend.Common.Params;
using Backend.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class RevisionsController(IRevisionService revisionService) : BaseApiController
{
	[Authorize(Policy = "RequireModeratorRole")]
	[HttpGet("{id:int}")]
	[ProducesResponseType(200)]
	[ProducesResponseType(401)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetEntityRevision(int id)
		=> (await revisionService.GetEntityRevisionAsync(id)).ToActionResult();

	[Authorize(Policy = "RequireModeratorRole")]
	[HttpGet("pending")]
	[ProducesResponseType(200)]
	[ProducesResponseType(401)]
	public async Task<IActionResult> GetPendingEntityRevisions([FromQuery] RevisionParams revisionParams)
		=> Ok(await revisionService.GetPendingEntityRevisionsAsync(revisionParams, Response));
}
