using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

// This is a mock controller for testing purposes
public class BuggyController(DataContext context) : BaseApiController
{
  [Authorize]
  [HttpGet("auth")]
  [ProducesResponseType(200)]
  [ProducesResponseType(401)]
  public IActionResult GetAuth()
  {
    return Ok("secret text");
  }

  [HttpGet("not-found")]
  [ProducesResponseType(404)]
  public IActionResult GetNotFound()
  {
    var thing = context.Users.Find(-1); // produce not found user
    if (thing == null) return NotFound();

    return Ok(thing); // will never reach here intentionally
  }

  [HttpGet("server-error")]
  [ProducesResponseType(500)]
  public IActionResult GetServerError() // 5xx = server error
  {
    var thing = context.Users.Find(-1) ?? throw new Exception("A bad thing has happened"); // generate null reference error

    return Ok(thing); // will never reach here intentionally
  }

  [HttpGet("bad-request")]
  [ProducesResponseType(400)]
  public IActionResult GetBadRequest() // 4xx = user error
  {
    return BadRequest("This was not a good request");
  }
}
