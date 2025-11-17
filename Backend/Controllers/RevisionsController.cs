using Backend.Common.Params;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class RevisionsController(IRevisionService revisionService) : BaseApiController
{
	[Authorize(Policy = "RequireModeratorRole")]
	[HttpGet("pending")]
	[ProducesResponseType(200)]
	[ProducesResponseType(401)]
	public async Task<IActionResult> GetPendingEntityRevisions([FromQuery] RevisionParams revisionParams) // TODO: Add RevisionParams for pagination
		=> Ok(await revisionService.GetPendingEntityRevisionsAsync(revisionParams, Response));
}
