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
  public async Task<IActionResult> CreateInitialRoasterRevision(CreateInitialRoasterRevisionDto createInitialRoasterRevisionDto)
    => (await roastersService.CreateInitialRoasterRevisionAsync(createInitialRoasterRevisionDto, User)).ToActionResult();

  [Authorize]
  [HttpPost("{id:int}/create-revision")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> CreateRoasterRevision(int id, CreateRoasterRevisionDto createRoasterRevision)
    => (await roastersService.CreateRoasterRevisionAsync(id, createRoasterRevision, User)).ToActionResult();

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
    => (await roastersService.GetRoasterRevisionExcerptsAsync(revisionExcerptParams, roasterId, Response, User)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("revisions/{revisionId:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> GetRevisionSnapshot(int revisionId)
    => (await roastersService.GetRoasterRevisionSnapshotAsync(revisionId, User)).ToActionResult();

  [AllowAnonymous]
  [HttpGet("{roasterId:int}/revisions/diff")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)] // If one of the revisionIds dont exist
  public async Task<IActionResult> GetRevisionDiff([FromQuery] int revisionId1, [FromQuery] int revisionId2, int roasterId)
    => (await roastersService.GetRoasterRevisionDiffAsync(revisionId1, revisionId2, roasterId, User)).ToActionResult();

  // [Authorize(Policy = "RequireModeratorRole")]
  // [HttpPost("{roasterId:int}/revisions/{revisionId:int}/approve")]
  // // if theres no parent, then roasterId will be null since it hasnt been created yet
  // // just make a 1 input overload for ApproveRoasterRevisionAsync
  // public async Task<IActionResult> ApproveRoasterRevision(int roasterId, int revisionId)
  //   => (await roastersService.ApproveRoasterRevisionAsync(roasterId, revisionId)).ToActionResult();

  // [Authorize(Policy = "RequireModeratorRole")]
  // [HttpPost("{roasterId:int}/revisions/{revisionId:int}/approve")]
  // public async Task<IActionResult> ApproveRoasterRevision(int roasterId, int revisionId)
  //   => (await roastersService.ApproveRoasterRevisionAsync(roasterId, revisionId)).ToActionResult();
}
