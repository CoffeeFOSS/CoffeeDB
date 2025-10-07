using Backend.Data;
using Backend.Entities;
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
  public ActionResult<string> GetAuth()
  {
    return "secret text";
  }

  [HttpGet("not-found")]
  [ProducesResponseType(404)]
  public ActionResult<User> GetNotFound()
  {
    var thing = context.Users.Find(-1); // produce not found user
    if (thing == null) return NotFound();
    return thing;
  }

  [HttpGet("server-error")]
  [ProducesResponseType(500)]
  public ActionResult<User> GetServerError() // 5xx = server error
  {
    var thing = context.Users.Find(-1) ?? throw new Exception("A bad thing has happened"); // generate null reference error

    return thing;
  }

  [HttpGet("bad-request")]
  [ProducesResponseType(400)]
  public ActionResult<string> GetBadRequest() // 4xx = user error
  {
    return BadRequest("This was not a good request");
  }
}
