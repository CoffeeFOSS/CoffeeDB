using Backend.Data;
using Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

public class UsersController(DataContext context) : BaseApiController
{
    [AllowAnonymous] // doesnt need to be here, just being explicit
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();

        return users;
    }

    [Authorize] // TODO: make allowanonymous later, just using this to test
    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        Console.WriteLine("AAAAAAAAAAAA");
        var user = await context.Users.FindAsync(id);

        if (user == null) return NotFound();

        return user;
    }
}
