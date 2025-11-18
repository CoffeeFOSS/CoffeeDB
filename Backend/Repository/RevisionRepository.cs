using Backend.Common;
using Backend.Common.Params;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities.Revision;
using Backend.Enums;
using Backend.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class RevisionRepository(DataContext context) : BaseRepository<EntityRevision>(context), IRevisionRepository
{
  public async Task<EntityRevisionDto?> GetEntityRevisionAsync(int id)
  {
    return await Context.EntityRevisions
      .Where(er => er.Id == id)
      .Select(er => new EntityRevisionDto
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

  public async Task<PagedList<EntityRevisionDto>> GetPendingEntityRevisionsAsync(RevisionParams revisionParams)
  {
    var query = Context.EntityRevisions
      .Include(er => er.CreatedBy)
      .Include(er => er.UpdatedBy)
      .AsQueryable();

    var dtoQuery = query
      .Where(er => er.Status == RevisionStatus.Pending)
      .Select(er => new EntityRevisionDto
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

    return await PagedList<EntityRevisionDto>.CreateAsync(dtoQuery, revisionParams.Page, revisionParams.PageSize);
  }

  public async Task<bool> ApprovePendingEntityRevisionAsync(int id, int approverUserId)
  {
    var entityRevision = await Context.EntityRevisions
      .Where(er => er.Id == id)
      .SingleOrDefaultAsync();

    if (entityRevision == null) return false;

    entityRevision.Status = RevisionStatus.Committed;
    entityRevision.UpdatedAt = DateTime.UtcNow;
    entityRevision.UpdatedById = approverUserId;

    return await SaveAllAsync();
  }

  public async Task<bool> RejectPendingEntityRevisionAsync(int id, int rejecterUserId)
  {
    var entityRevision = await Context.EntityRevisions
      .Where(er => er.Id == id)
      .SingleOrDefaultAsync();

    if (entityRevision == null) return false;

    entityRevision.Status = RevisionStatus.Rejected;
    entityRevision.UpdatedAt = DateTime.UtcNow;
    entityRevision.UpdatedById = rejecterUserId;

    return await SaveAllAsync();
  }

  public async Task<bool> AdoptPendingEntityRevisionsAsync(int oldParentRevisionId, int newParentRevisionId, int approverUserId)
  {
    var childRevisions = await Context.EntityRevisions
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

  public async Task<bool> EntityRevisionExists(int id)
  {
    return await Context.EntityRevisions.AnyAsync(er => er.Id == id);
  }
}
