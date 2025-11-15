using Backend.Common.Params;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class RevisionsController(IRevisionService revisionService) : BaseApiController
{
	[Authorize(Policy = "RequireModeratorRole")]
	[HttpGet()]
	[ProducesResponseType(200)]
	[ProducesResponseType(401)]
	public async Task<IActionResult> GetEntityRevisions([FromQuery] RevisionParams revisionParams) // TODO: Add RevisionParams for pagination
		=> Ok(await revisionService.GetEntityRevisionsAsync(revisionParams, Response));
}
