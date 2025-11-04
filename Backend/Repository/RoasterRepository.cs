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
        LocationAddress = r.LocationAddress,
        LocationCoordinates = GeoUtils.ToCoordinatesDto(r.LocationCoordinates),
        WebsiteUrl = r.WebsiteUrl,
        Description = r.Description
      })
      .SingleOrDefaultAsync();
  }

  public async Task<PagedList<RoasterDto>> GetRoastersAsync(RoasterParams roasterParams)
  {
    var query = Context.Roasters.AsQueryable();

    if (!string.IsNullOrWhiteSpace(roasterParams.Name))
    {
      var normalizedName = roasterParams.Name.ToLower();
      query = query
        .Where(r => r.Name.ToLower().Contains(normalizedName) || (r.Alias != null && r.Alias.ToLower().Contains(normalizedName)));
    }

    if (!string.IsNullOrWhiteSpace(roasterParams.Address))
    {
      query = query
        .Where(r => r.LocationAddress != null && r.LocationAddress.ToLower().Contains(roasterParams.Address.ToLower()));
    }

    NetTopologySuite.Geometries.Point? searchPoint = null;
    float? distanceInMeters = null;

    // TODO: Should we add a validation to ensure all 3 exists
    if (roasterParams.Lat.HasValue &&
        roasterParams.Long.HasValue &&
        roasterParams.Radius.HasValue)
    {
      searchPoint = GeoUtils.CreatePoint(
          roasterParams.Lat.Value,
          roasterParams.Long.Value
      );
      distanceInMeters = roasterParams.Radius * 1000;

      query = query.Where(r => r.LocationCoordinates != null && r.LocationCoordinates.Distance(searchPoint) <= distanceInMeters);
    }

    var dtoQuery = query.Select(r => new RoasterDto
    {
      Id = r.Id,
      Name = r.Name,
      Alias = r.Alias,
      LocationAddress = r.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(r.LocationCoordinates),
      WebsiteUrl = r.WebsiteUrl,
      Description = r.Description,
      DistanceInKilometers = searchPoint != null && r.LocationCoordinates != null
            ? Math.Round(r.LocationCoordinates.Distance(searchPoint) / 1000, 3)
            : null
    });
    dtoQuery = dtoQuery.OrderByDescending(r => r.Id);

    return await PagedList<RoasterDto>.CreateAsync(dtoQuery, roasterParams.Page, roasterParams.PageSize);
  }

  public async Task<RoasterDto?> CreateRoasterAsync(CreateRoasterDto createRoasterDto)
  {
    var roaster = new Roaster
    {
      Name = createRoasterDto.Name,
      Alias = createRoasterDto.Alias,
      LocationAddress = createRoasterDto.LocationAddress,
      LocationCoordinates = (createRoasterDto.LocationCoordinateLatitude.HasValue && createRoasterDto.LocationCoordinateLongitude.HasValue)
        ? GeoUtils.CreatePoint(createRoasterDto.LocationCoordinateLatitude.Value, createRoasterDto.LocationCoordinateLongitude.Value)
        : null,
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
      LocationAddress = roaster.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roaster.LocationCoordinates),
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
    roaster.LocationAddress = updateRoasterDto.LocationAddress ?? roaster.LocationAddress;
    roaster.LocationCoordinates = (updateRoasterDto.LocationCoordinateLatitude.HasValue && updateRoasterDto.LocationCoordinateLongitude.HasValue)
        ? GeoUtils.CreatePoint(updateRoasterDto.LocationCoordinateLatitude.Value, updateRoasterDto.LocationCoordinateLongitude.Value)
        : null;
    roaster.WebsiteUrl = updateRoasterDto.WebsiteUrl ?? roaster.WebsiteUrl;
    roaster.Description = updateRoasterDto.Description ?? roaster.Description;

    if (!await SaveAllAsync()) return null;

    return new RoasterDto
    {
      Id = roaster.Id,
      Name = roaster.Name,
      Alias = roaster.Alias,
      LocationAddress = roaster.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roaster.LocationCoordinates),
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

  public async Task<bool> RoasterExistsAsync(string name, string? locationAddress, int? excludeId = null)
  {
    string normalizedName = name.ToLower();

    var query = Context.Roasters.Where(r =>
      r.Name.ToLower() == normalizedName && (
        (locationAddress == null && r.LocationAddress == null) ||
        (r.LocationAddress != null && locationAddress != null && r.LocationAddress.ToLower() == locationAddress.ToLower())
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
