using System.Reflection;
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
        .Where(r =>
          r.Name.ToLower().Contains(normalizedName) ||
          (r.Alias != null && r.Alias.ToLower().Contains(normalizedName))
        );
    }

    if (!string.IsNullOrWhiteSpace(roasterParams.Address))
    {
      query = query.Where(r => r.LocationAddress != null && r.LocationAddress.ToLower().Contains(roasterParams.Address.ToLower()));
    }

    NetTopologySuite.Geometries.Point? searchPoint = null;
    double? distanceInMeters = null;

    if (roasterParams.Lat.HasValue &&
        roasterParams.Long.HasValue &&
        roasterParams.Radius.HasValue)
    {
      searchPoint = GeoUtils.CreatePoint(roasterParams.Lat.Value, roasterParams.Long.Value);
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

  public async Task<Roaster> CreateRoasterAsync(RoasterRevision roasterRevision)
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
    roasterRevision.Roaster = roaster;

    return roaster;
  }

  public async Task<Roaster?> UpdateRoasterAsync(RoasterRevision roasterRevision)
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

    return roaster;
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
    {
      query = query.Where(r => r.Id != excludeId.Value);
    }

    return await query.AnyAsync();
  }

  public async Task<bool> RoasterExistsByIdAsync(int id)
  {
    return await Context.Roasters.AnyAsync(r => r.Id == id);
  }

  public async Task<PagedList<RevisionMetadataExcerptDto>> GetRoasterRevisionExcerptsAsync(RevisionParams revisionExcerptParams, int roasterId, bool userIsModerator = false)
  {
    var query = Context.RoasterRevisions.AsQueryable();

    if (!userIsModerator || revisionExcerptParams.CommittedOnly == true)
    {
      query = query.Where(rr => rr.RevisionMetadata.Status == RevisionStatus.Committed);
    }

    var dtoQuery = query
      .OrderByDescending(rr => rr.Id)
      .Where(rr => rr.RoasterId == roasterId)
      .Select(rr => new RevisionMetadataExcerptDto
      {
        Id = rr.Id,
        Comment = rr.RevisionMetadata.Comment,
        Version = rr.RevisionMetadata.Version,
        ParentRevisionId = rr.RevisionMetadata.ParentRevisionId,
        Status = rr.RevisionMetadata.Status.ToString(),
        CreatedAt = rr.RevisionMetadata.CreatedAt,
        UpdatedAt = rr.RevisionMetadata.UpdatedAt,
        CreatedBy = rr.RevisionMetadata.CreatedBy == null ? null : rr.RevisionMetadata.CreatedBy.UserName
      });

    return await PagedList<RevisionMetadataExcerptDto>.CreateAsync(dtoQuery, revisionExcerptParams.Page, revisionExcerptParams.PageSize);
  }

  public async Task<PagedList<RevisionMetadataExcerptDto>> GetNewRoasterRevisionExcerptsAsync(RevisionParams revisionExcerptParams)
  {
    var query = Context.RoasterRevisions.AsQueryable();

    var dtoQuery = query
      .OrderByDescending(rr => rr.Id)
      .Where(rr => rr.RevisionMetadata.Status == RevisionStatus.Pending && rr.RoasterId == null)
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

  public async Task<RoasterRevisionSnapshotDto?> GetRoasterRevisionSnapshotAsync(int revisionId, bool userIsModerator, int? userId = null)
  {
    var rr = await Context.RoasterRevisions
      .Include(r => r.RevisionMetadata)
        .ThenInclude(rm => rm.CreatedBy)
      .Include(r => r.RevisionMetadata)
        .ThenInclude(rm => rm.UpdatedBy)
      .Where(rr => rr.Id == revisionId)
      .Select(rr => new
      {
        Revision = rr,
        RevisionStatus = rr.RevisionMetadata.Status,
        RevisionCreatorId = rr.RevisionMetadata.CreatedById,
      })
      .SingleOrDefaultAsync();

    if (rr == null) return null;

    bool isStatusCheckRequired = !userIsModerator && rr.RevisionStatus != RevisionStatus.Committed;
    bool isUserMismatch = userId == null || rr.RevisionCreatorId != userId;
    if (isStatusCheckRequired && isUserMismatch) return null;

    return new RoasterRevisionSnapshotDto
    {
      Id = rr.Revision.Id,
      Version = rr.Revision.RevisionMetadata.Version,
      RoasterId = rr.Revision.RoasterId,
      ParentRevisionId = rr.Revision.RevisionMetadata.ParentRevisionId,
      Status = rr.Revision.RevisionMetadata.Status.ToString(),
      Name = rr.Revision.Name,
      Alias = rr.Revision.Alias,
      LocationAddress = rr.Revision.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(rr.Revision.LocationCoordinates),
      WebsiteUrl = rr.Revision.WebsiteUrl,
      Description = rr.Revision.Description,
      Comment = rr.Revision.RevisionMetadata.Comment,
      CreatedAt = rr.Revision.RevisionMetadata.CreatedAt,
      UpdatedAt = rr.Revision.RevisionMetadata.UpdatedAt,
      CreatedBy = rr.Revision.RevisionMetadata.CreatedBy?.UserName,
      CreatedById = rr.Revision.RevisionMetadata.CreatedBy?.Id,
      UpdatedBy = rr.Revision.RevisionMetadata.UpdatedBy?.UserName,
      UpdatedById = rr.Revision.RevisionMetadata.UpdatedBy?.Id,
    };
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
        Version = rr.RevisionMetadata.Version,
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
      Alias = string.IsNullOrWhiteSpace(createRoasterRevisionDto.Alias) ? null : createRoasterRevisionDto.Alias,
      LocationAddress = string.IsNullOrWhiteSpace(createRoasterRevisionDto.LocationAddress) ? null : createRoasterRevisionDto.LocationAddress,
      LocationCoordinates = (createRoasterRevisionDto.LocationCoordinateLatitude.HasValue && createRoasterRevisionDto.LocationCoordinateLongitude.HasValue)
        ? GeoUtils.CreatePoint(createRoasterRevisionDto.LocationCoordinateLatitude.Value, createRoasterRevisionDto.LocationCoordinateLongitude.Value)
        : null,
      WebsiteUrl = string.IsNullOrWhiteSpace(createRoasterRevisionDto.WebsiteUrl) ? null : createRoasterRevisionDto.WebsiteUrl,
      Description = string.IsNullOrWhiteSpace(createRoasterRevisionDto.Description) ? null : createRoasterRevisionDto.Description,
    };

    Context.RoasterRevisions.Add(roasterRevision);

    var result = await SaveAllAsync();
    if (!result) return null;

    return new RoasterRevisionSnapshotDto
    {
      // CreatedBy, UpdatedBy will both be null at this point because EF hasn't pulled revisionMetadata data from DB
      Id = roasterRevision.Id,
      RoasterId = roasterRevision.RoasterId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      Name = roasterRevision.Name,
      Alias = roasterRevision.Alias,
      LocationAddress = roasterRevision.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roasterRevision.LocationCoordinates),
      WebsiteUrl = roasterRevision.WebsiteUrl,
      Description = roasterRevision.Description,
    };
  }

  public async Task<RoasterRevisionSnapshotDto?> CreateRoasterRevisionAsync(int roasterId, RevisionMetadataDto revisionMetadata, CreateRoasterRevisionDto createRoasterRevisionDto)
  {
    var roasterRevision = new RoasterRevision(revisionMetadata.Id)
    {
      RoasterId = roasterId,
      RevisionMetadataId = revisionMetadata.Id,
      Name = createRoasterRevisionDto.Name,
      Alias = string.IsNullOrWhiteSpace(createRoasterRevisionDto.Alias) ? null : createRoasterRevisionDto.Alias,
      LocationAddress = string.IsNullOrWhiteSpace(createRoasterRevisionDto.LocationAddress) ? null : createRoasterRevisionDto.LocationAddress,
      LocationCoordinates = (createRoasterRevisionDto.LocationCoordinateLatitude.HasValue && createRoasterRevisionDto.LocationCoordinateLongitude.HasValue)
        ? GeoUtils.CreatePoint(createRoasterRevisionDto.LocationCoordinateLatitude.Value, createRoasterRevisionDto.LocationCoordinateLongitude.Value)
        : null,
      WebsiteUrl = string.IsNullOrWhiteSpace(createRoasterRevisionDto.WebsiteUrl) ? null : createRoasterRevisionDto.WebsiteUrl,
      Description = string.IsNullOrWhiteSpace(createRoasterRevisionDto.Description) ? null : createRoasterRevisionDto.Description,
    };

    Context.RoasterRevisions.Add(roasterRevision);

    var result = await SaveAllAsync();
    if (!result) return null;

    return new RoasterRevisionSnapshotDto
    {
      // CreatedBy, UpdatedBy will both be null at this point because EF hasn't pulled revisionMetadata data from DB
      Id = roasterRevision.Id,
      RoasterId = roasterRevision.RoasterId,
      Comment = revisionMetadata.Comment,
      Version = revisionMetadata.Version,
      Status = revisionMetadata.Status.ToString(),
      ParentRevisionId = revisionMetadata.ParentRevisionId,
      Name = roasterRevision.Name,
      Alias = roasterRevision.Alias,
      LocationAddress = roasterRevision.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roasterRevision.LocationCoordinates),
      WebsiteUrl = roasterRevision.WebsiteUrl,
      Description = roasterRevision.Description,
    };
  }

  public async Task<RoasterRevisionSnapshotDto?> UpdateRoasterRevisionAsync(int revisionId, CreateRoasterRevisionDto createRoasterRevisionDto)
  {
    var roasterRevision = await Context.RoasterRevisions
      .Where(r => r.Id == revisionId)
      .SingleOrDefaultAsync();

    if (roasterRevision == null) return null;

    roasterRevision.Name = createRoasterRevisionDto.Name;
    roasterRevision.Alias = createRoasterRevisionDto.Alias;
    roasterRevision.LocationAddress = createRoasterRevisionDto.LocationAddress;
    roasterRevision.LocationCoordinates = (createRoasterRevisionDto.LocationCoordinateLatitude.HasValue && createRoasterRevisionDto.LocationCoordinateLongitude.HasValue)
        ? GeoUtils.CreatePoint(createRoasterRevisionDto.LocationCoordinateLatitude.Value, createRoasterRevisionDto.LocationCoordinateLongitude.Value)
        : null;
    roasterRevision.WebsiteUrl = createRoasterRevisionDto.WebsiteUrl;
    roasterRevision.Description = createRoasterRevisionDto.Description;

    if (!await SaveAllAsync()) return null;

    return new RoasterRevisionSnapshotDto
    {
      Id = roasterRevision.Id,
      RoasterId = roasterRevision.RoasterId,
      Comment = roasterRevision.RevisionMetadata.Comment,
      Version = roasterRevision.RevisionMetadata.Version,
      Status = roasterRevision.RevisionMetadata.Status.ToString(),
      ParentRevisionId = roasterRevision.RevisionMetadata.ParentRevisionId,
      Name = roasterRevision.Name,
      Alias = roasterRevision.Alias,
      LocationAddress = roasterRevision.LocationAddress,
      LocationCoordinates = GeoUtils.ToCoordinatesDto(roasterRevision.LocationCoordinates),
      WebsiteUrl = roasterRevision.WebsiteUrl,
      Description = roasterRevision.Description,
      CreatedAt = roasterRevision.RevisionMetadata.CreatedAt,
      CreatedBy = roasterRevision.RevisionMetadata.CreatedBy?.UserName,
      CreatedById = roasterRevision.RevisionMetadata.CreatedBy?.Id,
      UpdatedAt = roasterRevision.RevisionMetadata.UpdatedAt,
      UpdatedBy = roasterRevision.RevisionMetadata.UpdatedBy?.UserName,
      UpdatedById = roasterRevision.RevisionMetadata.UpdatedBy?.Id,
    };
  }
}
