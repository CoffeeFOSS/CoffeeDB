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
  [HttpPatch("update/{id:int}")]
  [ProducesResponseType(200)]
  [ProducesResponseType(400)]
  [ProducesResponseType(500)]
  public async Task<IActionResult> UpdateRoaster(int id, UpdateRoasterDto updateRoasterDto)
    => (await roastersService.UpdateRoasterAsync(id, updateRoasterDto)).ToActionResult();
}
