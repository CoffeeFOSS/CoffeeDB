using Backend.Common;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces;

namespace Backend.Repository;

public class RoasterRepository(DataContext context) : IRoasterRepository
{
  public Task<RoasterDto?> GetRoasterById(int id)
  {
    throw new NotImplementedException();
  }

  public async Task<PagedList<RoasterDto>> GetRoastersAsync(UserParams userParams)
  {
    var query = context.Roasters
      .Select(r => new RoasterDto
      {
        Id = r.Id,
        Name = r.Name,
        Alias = r.Alias,
        Location = r.Location,
        WebsiteUrl = r.WebsiteUrl,
        Description = r.Description,
      });

    return await PagedList<RoasterDto>.CreateAsync(query, userParams.Page, userParams.PageSize);
  }

  public Task<bool> SaveAllAsync()
  {
    throw new NotImplementedException();
  }

  public void Update(Roaster roaster)
  {
    throw new NotImplementedException();
  }
}
