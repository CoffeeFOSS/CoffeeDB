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
}
