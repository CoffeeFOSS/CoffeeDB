using Backend.Common.Params;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class RoastersController(IRoasterService roastersService) : BaseApiController
{
  [AllowAnonymous]
  [HttpGet]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRoasters([FromQuery] RoasterParams roasterParams)
    => Ok(await roastersService.GetRoastersAsync(roasterParams, Response));

  [AllowAnonymous]
  [HttpGet("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetRoaster(int id)
    => (await roastersService.GetRoasterAsync(id)).ToActionResult();

  [Authorize]
  [HttpPost("create")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateRoaster(CreateRoasterDto createRoasterDto)
    => (await roastersService.CreateRoasterAsync(createRoasterDto)).ToActionResult();

  [Authorize]
  [HttpPatch("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> UpdateRoaster(int id, UpdateRoasterDto updateRoasterDto)
    => (await roastersService.UpdateRoasterAsync(id, updateRoasterDto)).ToActionResult();

  [Authorize(Policy = "RequireAdminRole")]
  [HttpDelete("{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> DeleteRoaster(int id)
    => (await roastersService.DeleteRoasterAsync(id)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{roasterId:int}/revisions")]
  [ProducesResponseType(200)]
  public async Task<IActionResult> GetRevisionExcerpts([FromQuery] RevisionParams revisionExcerptParams, int roasterId)
    // Should only get Revision ID, Name, Comment. Only show Committed revisions
    => (await roastersService.GetRoasterRevisionExcerptsAsync(revisionExcerptParams, roasterId, Response)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{roasterId:int}/revisions/snapshot/{revisionId:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetRevisionSnapshot(int revisionId, int roasterId)
    // Should get exact snapshot of this revision. Do not show if the id is for a non-committed revision
    => (await roastersService.GetRoasterRevisionSnapshotAsync(revisionId, roasterId)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{roasterId:int}/revisions/diff")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)] // If one of the revisionIds dont exist
  public async Task<IActionResult> GetRevisionDiff([FromQuery] int revisionId1, [FromQuery] int revisionId2, int roasterId)
    // Should get diff between two revisions. Do not show if either id is for a non-committed revision
    // Always show the older one on the Old property, and new on New property, regardless of the order theyre provided in the query
    => (await roastersService.GetRoasterRevisionDiffAsync(revisionId1, revisionId2, roasterId)).ToActionResult();
}
