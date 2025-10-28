using Backend.Common;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class RoastersController(IRoasterService roastersService) : BaseApiController
{
  [AllowAnonymous]
  public async Task<IActionResult> GetRoasters([FromQuery] UserParams userParams)
    => Ok(await roastersService.GetRoastersAsync(userParams, Response));
}
