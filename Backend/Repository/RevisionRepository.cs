using Backend.Common;
using Backend.Common.Params;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities.Revision;
using Backend.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class RevisionRepository(DataContext context) : BaseRepository<EntityRevision>(context), IRevisionRepository
{
  public async Task<PagedList<EntityRevisionDto>> GetEntityRevisionsAsync(RevisionParams revisionParams)
  {
    var query = Context.EntityRevisions
      .Include(er => er.CreatedBy)
      .Include(er => er.UpdatedBy)
      .AsQueryable();

    var dtoQuery = query.Select(er => new EntityRevisionDto
    {
      Id = er.Id,
      Status = er.Status.ToString(),
      ParentRevisionId = er.ParentRevisionId,
      Version = er.Version,
      Comment = er.Comment,
      EntityType = er.GetType().Name,
      CreatedAt = er.CreatedAt,
      CreatedBy = er.CreatedBy,
      UpdatedAt = er.UpdatedAt,
      UpdatedBy = er.UpdatedBy,
    });

    dtoQuery = dtoQuery.OrderByDescending(r => r.Id);

    return await PagedList<EntityRevisionDto>.CreateAsync(dtoQuery, revisionParams.Page, revisionParams.PageSize);
  }
}
