using Backend.Common;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class RoasterRepository(DataContext context) : BaseRepository<Roaster>(context), IRoasterRepository
{
  public async Task<RoasterDto?> GetRoasterByIdAsync(int id)
  {
    return await Context.Roasters
      .Where(r => r.Id == id)
      .Select(r => new RoasterDto
      {
        Id = r.Id,
        Name = r.Name,
        Alias = r.Alias,
        Location = r.Location,
        WebsiteUrl = r.WebsiteUrl,
        Description = r.Description
      })
      .SingleOrDefaultAsync();
  }

  public async Task<PagedList<RoasterDto>> GetRoastersAsync(RoasterParams roasterParams)
  {
    var query = Context.Roasters
      .Select(r => new RoasterDto
      {
        Id = r.Id,
        Name = r.Name,
        Alias = r.Alias,
        Location = r.Location,
        WebsiteUrl = r.WebsiteUrl,
        Description = r.Description,
      });

    return await PagedList<RoasterDto>.CreateAsync(query, roasterParams.Page, roasterParams.PageSize);
  }

  public Task<RoasterDto?> CreateRoasterAsync()
  {
    throw new NotImplementedException();
  }

  public Task<RoasterDto?> UpdateRoasterAsync()
  {
    throw new NotImplementedException();
  }

  public Task<bool> DeleteRoasterAsync()
  {
    throw new NotImplementedException();
  }
}
