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

  public async Task<RoasterDto?> CreateRoasterAsync(RoasterRevision roasterRevision)
  {
    var roaster = new Roaster
    {
      Name = roasterRevision.Name,
      Alias = roasterRevision.Alias,
      LocationAddress = roasterRevision.LocationAddress,
      LocationCoordinates = roasterRevision.LocationCoordinates,
      WebsiteUrl = roasterRevision.WebsiteUrl,
      Description = roasterRevision.Description,
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

  public async Task<RoasterDto?> UpdateRoasterAsync(RoasterRevision roasterRevision)
  {
    var roaster = await Context.Roasters
      .Where(r => r.Id == roasterRevision.RoasterId)
      .SingleOrDefaultAsync();

    if (roaster == null) return null;

    roaster.Name = roasterRevision.Name;
    roaster.Alias = roasterRevision.Alias;
    roaster.LocationAddress = roasterRevision.LocationAddress;
    roaster.LocationCoordinates = roasterRevision.LocationCoordinates;
    roaster.WebsiteUrl = roasterRevision.WebsiteUrl;
    roaster.Description = roasterRevision.Description;

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

  public async Task<PagedList<RevisionMetadataExcerptDto>> GetRoasterRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int roasterId, bool ignoreStatus = false)
  {
    var query = Context.RoasterRevisions.AsQueryable();

    if (!ignoreStatus)
    {
      query = query.Where(rr => rr.RevisionMetadata.Status == RevisionStatus.Committed);
    }

    var dtoQuery = query
      .OrderByDescending(rr => rr.Id)
      .Select(rr => new RevisionMetadataExcerptDto
      {
        Id = rr.Id,
        Comment = rr.RevisionMetadata.Comment,
        Version = rr.RevisionMetadata.Version,
        ParentRevisionId = rr.RevisionMetadata.ParentRevisionId,
        Status = rr.RevisionMetadata.Status.ToString()
      });

    return await PagedList<RevisionMetadataExcerptDto>.CreateAsync(dtoQuery, revisionExcerptParams.Page, revisionExcerptParams.PageSize);
  }

  public async Task<RoasterRevisionSnapshotDto?> GetRoasterRevisionSnapshotAsync(int revisionId, bool ignoreStatus)
  {
    var query = Context.RoasterRevisions.AsQueryable();

    if (!ignoreStatus)
    {
      query = query.Where(rr => rr.RevisionMetadata.Status == RevisionStatus.Committed);
    }

    return await query
      .Where(rr => rr.Id == revisionId)
      .Select(rr => new RoasterRevisionSnapshotDto
      {
        Id = rr.Id,
        RoasterId = rr.RoasterId,
        Status = rr.RevisionMetadata.Status.ToString(),
        Name = rr.Name,
        Alias = rr.Alias,
        LocationAddress = rr.LocationAddress,
        LocationCoordinates = GeoUtils.ToCoordinatesDto(rr.LocationCoordinates),
        WebsiteUrl = rr.WebsiteUrl,
        Description = rr.Description,
        Comment = rr.RevisionMetadata.Comment,
        CreatedAt = rr.RevisionMetadata.CreatedAt,
        UpdatedAt = rr.RevisionMetadata.UpdatedAt,
        CreatedBy = rr.RevisionMetadata.CreatedBy == null ? null : rr.RevisionMetadata.CreatedBy.UserName,
        UpdatedBy = rr.RevisionMetadata.UpdatedBy == null ? null : rr.RevisionMetadata.UpdatedBy.UserName,
      })
      .SingleOrDefaultAsync();
  }

  public async Task<RoasterRevision?> GetRoasterRevisionEntityAsync(int revisionId)
  {
    return await Context.RoasterRevisions.SingleOrDefaultAsync(r => r.Id == revisionId);
  }

  public async Task<RevisionMetadataVersioningDto?> GetLatestRoasterRevisionVersionAsync(int roasterId)
  {
    var query = Context.RoasterRevisions.AsQueryable();

    return await query
      .Where(rr => rr.RoasterId == roasterId && rr.RevisionMetadata.Version != null)
      .OrderByDescending(rr => rr.RevisionMetadata.Version) // Version should never be null when Committed
      .Select(rr => new RevisionMetadataVersioningDto
      {
        Id = rr.Id,
      })
      .FirstOrDefaultAsync();
  }

  public async Task<RoasterRevisionSnapshotDto?> CreateInitialRoasterRevisionAsync(
    RevisionMetadataDto revisionMetadata,
    CreateRoasterRevisionDto createRoasterRevisionDto)
  {
    var roasterRevision = new RoasterRevision(revisionMetadata.Id)
    {
      RoasterId = null,
      RevisionMetadataId = revisionMetadata.Id,

      Name = createRoasterRevisionDto.Name,
      Alias = createRoasterRevisionDto.Alias,
      LocationAddress = createRoasterRevisionDto.LocationAddress,
      LocationCoordinates = (createRoasterRevisionDto.LocationCoordinateLatitude.HasValue && createRoasterRevisionDto.LocationCoordinateLongitude.HasValue)
        ? GeoUtils.CreatePoint(createRoasterRevisionDto.LocationCoordinateLatitude.Value, createRoasterRevisionDto.LocationCoordinateLongitude.Value)
        : null,
      WebsiteUrl = createRoasterRevisionDto.WebsiteUrl,
      Description = createRoasterRevisionDto.Description,
    };

    Context.RoasterRevisions.Add(roasterRevision);

    var result = await SaveAllAsync();
    if (!result) return null;

    return new RoasterRevisionSnapshotDto
    {
      Id = roasterRevision.Id,
      RoasterId = roasterRevision.RoasterId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      // CreatedBy, UpdatedBy will both be null at this point because 
      // EF hasn't pulled revisionMetadata data from DB

      Name = roasterRevision.Name,
      Alias = roasterRevision.Alias,
      LocationAddress = roasterRevision.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roasterRevision.LocationCoordinates),
      WebsiteUrl = roasterRevision.WebsiteUrl,
      Description = roasterRevision.Description,
    };
  }

  public async Task<RoasterRevisionSnapshotDto?> CreateRoasterRevisionAsync(int roasterId, RevisionMetadataDto revisionMetadata, CreateRoasterRevisionDto updateRoasterDto)
  {
    var roasterRevision = new RoasterRevision(revisionMetadata.Id)
    {
      RoasterId = roasterId,
      RevisionMetadataId = revisionMetadata.Id,

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
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      // CreatedBy, UpdatedBy will both be null at this point because 
      // EF hasn't pulled revisionMetadata data from DB

      Name = roasterRevision.Name,
      Alias = roasterRevision.Alias,
      LocationAddress = roasterRevision.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roasterRevision.LocationCoordinates),
      WebsiteUrl = roasterRevision.WebsiteUrl,
      Description = roasterRevision.Description,
    };
  }
}
