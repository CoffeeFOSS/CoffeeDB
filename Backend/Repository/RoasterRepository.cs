using Backend.Common;
using Backend.Common.Params;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Interfaces.Repository;
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

    // TODO: For the text searches below, we should use tsvector

    if (!string.IsNullOrWhiteSpace(roasterParams.Name))
    {
      var normalizedName = roasterParams.Name.ToLower();
      query = query
        .Where(r => r.Name.ToLower().Contains(normalizedName) || (r.Alias != null && r.Alias.ToLower().Contains(normalizedName)));
    }

    if (!string.IsNullOrWhiteSpace(roasterParams.Location))
    {
      query = query
        .Where(r => r.Location != null && r.Location.ToLower().Contains(roasterParams.Location.ToLower()));
    }

    query = query.OrderBy(r => r.Name);

    return await PagedList<RoasterDto>.CreateAsync(query, roasterParams.Page, roasterParams.PageSize);
  }

  public async Task<RoasterDto?> CreateRoasterAsync(CreateRoasterDto createRoasterDto)
  {
    var roaster = new Roaster
    {
      Name = createRoasterDto.Name,
      Alias = createRoasterDto.Alias,
      Location = createRoasterDto.Location,
      WebsiteUrl = createRoasterDto.WebsiteUrl,
      Description = createRoasterDto.Description,
    };

    Context.Roasters.Add(roaster);

    var result = await SaveAllAsync();
    if (!result)
    {
      return null;
    }

    return new RoasterDto
    {
      Id = roaster.Id,
      Name = roaster.Name,
      Alias = roaster.Alias,
      Location = roaster.Location,
      WebsiteUrl = roaster.WebsiteUrl,
      Description = roaster.Description,
    };
  }

  public async Task<RoasterDto?> UpdateRoasterAsync(int id, UpdateRoasterDto updateRoasterDto)
  {
    var roaster = await Context.Roasters
      .Where(r => r.Id == id)
      .SingleOrDefaultAsync();

    if (roaster == null) return null;

    roaster.Name = updateRoasterDto.Name ?? roaster.Name;
    roaster.Alias = updateRoasterDto.Alias ?? roaster.Alias;
    roaster.Location = updateRoasterDto.Location ?? roaster.Location;
    roaster.WebsiteUrl = updateRoasterDto.WebsiteUrl ?? roaster.WebsiteUrl;
    roaster.Description = updateRoasterDto.Description ?? roaster.Description;

    if (!await SaveAllAsync()) return null;

    return new RoasterDto
    {
      Id = roaster.Id,
      Name = roaster.Name,
      Alias = roaster.Alias,
      Location = roaster.Location,
      WebsiteUrl = roaster.WebsiteUrl,
      Description = roaster.Description,
    };
  }

  public async Task<bool> DeleteRoasterAsync(int id)
  {
    var roaster = await Context.Roasters
      .Where(r => r.Id == id)
      .SingleOrDefaultAsync();

    if (roaster == null) return false;

    Context.Roasters.Remove(roaster);

    return await SaveAllAsync();
  }

  public async Task<bool> RoasterExistsAsync(string name, string? location, int? excludeId = null)
  {
    string normalizedName = name.ToLower();

    var query = Context.Roasters.Where(r =>
      r.Name.ToLower() == normalizedName && (
        location == null || (r.Location != null && r.Location.ToLower() == location.ToLower())
      )
    );

    if (excludeId.HasValue)
      query = query.Where(r => r.Id != excludeId.Value);

    return await query.AnyAsync();
  }

  public async Task<bool> RoasterExistsByIdAsync(int id)
  {
    return await Context.Roasters.AnyAsync(r => r.Id == id);
  }
}
