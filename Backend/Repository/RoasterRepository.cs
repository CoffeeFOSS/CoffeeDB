using Backend.Common;
using Backend.Common.Params;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.Revision;
using Backend.Enums;
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
    double? distanceInMeters = null;

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

  public async Task<PagedList<RoasterRevisionExcerptDto>> GetRoasterRevisionExcerptsAsync(PaginationParams revisionExcerptParams, int roasterId, bool ignoreStatus = false)
  {
    var query = Context.RoasterRevisions.AsQueryable();

    if (!ignoreStatus)
    {
      query = query.Where(rr => rr.EntityRevision.Status == RevisionStatus.Committed);
    }

    var dtoQuery = query
      .OrderByDescending(rr => rr.Id)
      .Select(rr => new RoasterRevisionExcerptDto
      {
        Id = rr.Id,
        Comment = rr.EntityRevision.Comment,
        Version = rr.EntityRevision.Version,
        ParentRevisionId = rr.EntityRevision.ParentRevisionId,
        Status = ignoreStatus ? rr.EntityRevision.Status.ToString() : RevisionStatus.Committed.ToString()
      });

    return await PagedList<RoasterRevisionExcerptDto>.CreateAsync(dtoQuery, revisionExcerptParams.Page, revisionExcerptParams.PageSize);
  }

  public async Task<RoasterRevisionSnapshotDto?> GetRoasterRevisionSnapshotAsync(int revisionId, bool ignoreStatus)
  {
    var query = Context.RoasterRevisions.AsQueryable();

    if (!ignoreStatus)
    {
      query = query.Where(rr => rr.EntityRevision.Status == RevisionStatus.Committed);
    }

    return await query
      .Where(rr => rr.Id == revisionId)
      .Select(rr => new RoasterRevisionSnapshotDto
      {
        Id = rr.Id,
        RoasterId = rr.RoasterId,
        EntityRevisionId = rr.EntityRevisionId,
        Status = rr.EntityRevision.Status.ToString(),
        Name = rr.Name,
        Alias = rr.Alias,
        LocationAddress = rr.LocationAddress,
        LocationCoordinates = GeoUtils.ToCoordinatesDto(rr.LocationCoordinates),
        WebsiteUrl = rr.WebsiteUrl,
        Description = rr.Description,
        Comment = rr.EntityRevision.Comment,
        CreatedAt = rr.EntityRevision.CreatedAt,
        UpdatedAt = rr.EntityRevision.UpdatedAt,
        CreatedBy = rr.EntityRevision.CreatedBy == null ? null : rr.EntityRevision.CreatedBy.UserName,
        UpdatedBy = rr.EntityRevision.UpdatedBy == null ? null : rr.EntityRevision.UpdatedBy.UserName,
      })
      .SingleOrDefaultAsync();
  }

  public async Task<RoasterRevisionVersioningDto?> GetLatestRoasterRevisionVersionAsync(int roasterId)
  {
    var query = Context.RoasterRevisions.AsQueryable();

    return await query
      .Where(rr => rr.RoasterId == roasterId && rr.EntityRevision.Version != null)
      .OrderByDescending(rr => rr.EntityRevision.Version) // Version should never be null when Committed
      .Select(rr => new RoasterRevisionVersioningDto
      {
        Id = rr.Id,
      })
      .FirstOrDefaultAsync();
  }

  public async Task<RoasterRevisionSnapshotDto?> CreateRoasterRevisionAsync(int roasterId, EntityRevisionDto entityRevision, UpdateRoasterDto updateRoasterDto)
  {
    var roasterRevision = new RoasterRevision
    {
      RoasterId = roasterId,
      EntityRevisionId = entityRevision.Id,

      Name = updateRoasterDto.Name,
      Alias = updateRoasterDto.Alias,
      LocationAddress = updateRoasterDto.LocationAddress,
      LocationCoordinates = (updateRoasterDto.LocationCoordinateLatitude.HasValue && updateRoasterDto.LocationCoordinateLongitude.HasValue)
        ? GeoUtils.CreatePoint(updateRoasterDto.LocationCoordinateLatitude.Value, updateRoasterDto.LocationCoordinateLongitude.Value)
        : null,
      WebsiteUrl = updateRoasterDto.WebsiteUrl,
      Description = updateRoasterDto.Description,
    };

    Context.RoasterRevisions.Add(roasterRevision);

    var result = await SaveAllAsync();
    if (!result) return null;

    return new RoasterRevisionSnapshotDto
    {
      Id = roasterRevision.Id,
      RoasterId = roasterRevision.RoasterId,
      EntityRevisionId = roasterRevision.EntityRevisionId,
      Comment = entityRevision.Comment,
      Version = entityRevision.Version,
      Status = entityRevision.Status.ToString(),
      // CreatedBy, UpdatedBy will both be null at this point because 
      // EF hasn't pulled entityRevision data from DB

      Name = roasterRevision.Name,
      Alias = roasterRevision.Alias,
      LocationAddress = roasterRevision.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roasterRevision.LocationCoordinates),
      WebsiteUrl = roasterRevision.WebsiteUrl,
      Description = roasterRevision.Description,
    };
  }

  public async Task<EntityRevisionDto?> CreateEntityRevisionAsync(string comment, int userId, int? parentRevisionId)
  {
    var entityRevision = new EntityRevision
    {
      Status = RevisionStatus.Pending,
      ParentRevisionId = parentRevisionId,
      Comment = comment,
      CreatedAt = DateTime.UtcNow,
      CreatedById = userId
    };

    Context.EntityRevisions.Add(entityRevision);

    var result = await SaveAllAsync();
    if (!result) return null;

    return new EntityRevisionDto
    {
      Id = entityRevision.Id,
      Status = entityRevision.Status.ToString(),
      ParentRevisionId = entityRevision.ParentRevisionId,
      Version = entityRevision.Version,
      Comment = entityRevision.Comment,
      EntityType = EntityType.Roaster.ToString(),
    };
  }
}
