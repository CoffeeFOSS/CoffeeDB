using Backend.Common;
using Backend.Common.Params;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities.Revision;
using Backend.Enums;
using Backend.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository;

public class RevisionMetadataRepository(DataContext context) : BaseRepository<RevisionMetadata>(context), IRevisionMetadataRepository
{
  public async Task<RevisionMetadataDto?> GetRevisionMetadataAsync(int id)
  {
    return await Context.RevisionMetadatas
      .Where(rm => rm.Id == id)
      .Select(rm => new RevisionMetadataDto
      {
        Id = rm.Id,
        Status = rm.Status.ToString(),
        ParentRevisionId = rm.ParentRevisionId,
        Version = rm.Version,
        Comment = rm.Comment,
        EntityType =
          rm.RoasterRevision != null ? "Roaster" :
          // rm.AnotherEntityRevisions != null ? "AnotherEntity" :
          "Unknown",
        CreatedAt = rm.CreatedAt,
        CreatedBy = rm.CreatedBy == null ? null : rm.CreatedBy.UserName,
        UpdatedAt = rm.UpdatedAt,
        UpdatedBy = rm.UpdatedBy == null ? null : rm.UpdatedBy.UserName,
      })
      .SingleOrDefaultAsync();
  }

  public async Task<PagedList<RevisionMetadataDto>> GetPendingRevisionMetadatasAsync(RevisionParams revisionParams, bool userIsModerator, int? userId)
  {
    var query = Context.RevisionMetadatas
      .Include(rm => rm.CreatedBy)
      .Include(rm => rm.UpdatedBy)
      .AsQueryable();

    if (!userIsModerator && userId != null)
    {
      query = query.Where(rm => rm.CreatedById == userId);
    }

    var dtoQuery = query
      .Where(rm => rm.Status == RevisionStatus.Pending)
      .Select(rm => new RevisionMetadataDto
      {
        Id = rm.Id,
        Status = rm.Status.ToString(),
        ParentRevisionId = rm.ParentRevisionId,
        Version = rm.Version,
        Comment = rm.Comment,
        EntityType =
          rm.RoasterRevision != null ? "Roaster" :
          // rm.AnotherEntityRevisions != null ? "AnotherEntity" :
          "Unknown",
        CreatedAt = rm.CreatedAt,
        CreatedBy = rm.CreatedBy == null ? null : rm.CreatedBy.UserName,
        UpdatedAt = rm.UpdatedAt,
        UpdatedBy = rm.UpdatedBy == null ? null : rm.UpdatedBy.UserName,
      });

    dtoQuery = dtoQuery.OrderByDescending(r => r.Id);

    return await PagedList<RevisionMetadataDto>.CreateAsync(dtoQuery, revisionParams.Page, revisionParams.PageSize);
  }

  public async Task<RevisionMetadataDto?> CreateRevisionMetadataAsync(string comment, int userId, EntityType entityType, int? parentRevisionId)
  {
    var revisionMetadata = new RevisionMetadata
    {
      Status = RevisionStatus.Pending,
      ParentRevisionId = parentRevisionId,
      Comment = comment,
      CreatedAt = DateTime.UtcNow,
      CreatedById = userId
    };

    Context.RevisionMetadatas.Add(revisionMetadata);

    var result = await SaveAllAsync();
    if (!result) return null;

    return new RevisionMetadataDto
    {
      Id = revisionMetadata.Id,
      Status = revisionMetadata.Status.ToString(),
      ParentRevisionId = revisionMetadata.ParentRevisionId,
      Version = revisionMetadata.Version,
      Comment = revisionMetadata.Comment,
      EntityType = entityType.ToString(),
    };
  }

  public async Task<bool> ApprovePendingRevisionMetadataAsync(int id, int approverUserId, int? latestVersionId = null)
  {
    var revisionMetadata = await Context.RevisionMetadatas
      .Where(rm => rm.Id == id)
      .SingleOrDefaultAsync();

    if (revisionMetadata == null) return false;

    revisionMetadata.Status = RevisionStatus.Committed;
    revisionMetadata.UpdatedAt = DateTime.UtcNow;
    revisionMetadata.UpdatedById = approverUserId;

    if (latestVersionId == null) revisionMetadata.Version = 1;
    else if (latestVersionId <= 0) return false;
    else revisionMetadata.Version = latestVersionId + 1;

    return await SaveAllAsync();
  }

  public async Task<bool> RejectPendingRevisionMetadataAsync(int id, int rejecterUserId)
  {
    var revisionMetadata = await Context.RevisionMetadatas
      .Where(rm => rm.Id == id)
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
      .Where(rm => rm.ParentRevisionId == oldParentRevisionId && (rm.Status == RevisionStatus.Pending || rm.Status == RevisionStatus.Draft))
      .ToListAsync();

    if (childRevisions.Count == 0) return true;

    foreach (var childRevision in childRevisions)
    {
      childRevision.ParentRevisionId = newParentRevisionId;
      childRevision.UpdatedAt = DateTime.UtcNow;
      childRevision.UpdatedById = approverUserId;
    }

    return await SaveAllAsync();
  }
}
