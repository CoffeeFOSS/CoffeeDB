using Backend.Common;
using Backend.Common.Params;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities.Revision;
using Backend.Enums;
using Backend.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class RevisionRepository(DataContext context) : BaseRepository<RevisionMetadata>(context), IRevisionRepository
{
  public async Task<RevisionMetadataDto?> GetRevisionMetadataAsync(int id)
  {
    return await Context.RevisionMetadatas
      .Where(er => er.Id == id)
      .Select(er => new RevisionMetadataDto
      {
        Id = er.Id,
        Status = er.Status.ToString(),
        ParentRevisionId = er.ParentRevisionId,
        Version = er.Version,
        Comment = er.Comment,
        EntityType =
          er.RoasterRevisions.Any() ? "Roaster" :
          // er.AnotherEntityRevisions.Any() ? "AnotherEntity" :
          "Unknown",
        CreatedAt = er.CreatedAt,
        CreatedBy = er.CreatedBy == null ? null : er.CreatedBy.UserName,
        UpdatedAt = er.UpdatedAt,
        UpdatedBy = er.UpdatedBy == null ? null : er.UpdatedBy.UserName,
      })
      .SingleOrDefaultAsync();
  }

  public async Task<PagedList<RevisionMetadataDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams)
  {
    var query = Context.RevisionMetadatas
      .Include(er => er.CreatedBy)
      .Include(er => er.UpdatedBy)
      .AsQueryable();

    var dtoQuery = query
      .Where(er => er.Status == RevisionStatus.Pending)
      .Select(er => new RevisionMetadataDto
      {
        Id = er.Id,
        Status = er.Status.ToString(),
        ParentRevisionId = er.ParentRevisionId,
        Version = er.Version,
        Comment = er.Comment,
        EntityType =
          er.RoasterRevisions.Any() ? "Roaster" :
          // er.AnotherEntityRevisions.Any() ? "AnotherEntity" :
          "Unknown",
        CreatedAt = er.CreatedAt,
        CreatedBy = er.CreatedBy == null ? null : er.CreatedBy.UserName,
        UpdatedAt = er.UpdatedAt,
        UpdatedBy = er.UpdatedBy == null ? null : er.UpdatedBy.UserName,
      });

    dtoQuery = dtoQuery.OrderByDescending(r => r.Id);

    return await PagedList<RevisionMetadataDto>.CreateAsync(dtoQuery, revisionParams.Page, revisionParams.PageSize);
  }

  public async Task<bool> ApprovePendingRevisionMetadataAsync(int id, int approverUserId)
  {
    var revisionMetadata = await Context.RevisionMetadatas
      .Where(er => er.Id == id)
      .SingleOrDefaultAsync();

    if (revisionMetadata == null) return false;

    revisionMetadata.Status = RevisionStatus.Committed;
    revisionMetadata.UpdatedAt = DateTime.UtcNow;
    revisionMetadata.UpdatedById = approverUserId;

    return await SaveAllAsync();
  }

  public async Task<bool> RejectPendingRevisionMetadataAsync(int id, int rejecterUserId)
  {
    var revisionMetadata = await Context.RevisionMetadatas
      .Where(er => er.Id == id)
      .SingleOrDefaultAsync();

    if (revisionMetadata == null) return false;

    revisionMetadata.Status = RevisionStatus.Rejected;
    revisionMetadata.UpdatedAt = DateTime.UtcNow;
    revisionMetadata.UpdatedById = rejecterUserId;

    return await SaveAllAsync();
  }

  public async Task<bool> AdoptPendingRevisionMetadatasAsync(int oldParentRevisionId, int newParentRevisionId, int approverUserId)
  {
    var childRevisions = await Context.RevisionMetadatas
      .Where(er => er.ParentRevisionId == oldParentRevisionId && (er.Status == RevisionStatus.Pending || er.Status == RevisionStatus.Draft))
      .ToListAsync();

    foreach (var childRevision in childRevisions)
    {
      childRevision.ParentRevisionId = newParentRevisionId;
      childRevision.UpdatedAt = DateTime.UtcNow;
      childRevision.UpdatedById = approverUserId;
    }

    return await SaveAllAsync();
  }
}
