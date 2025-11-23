using Backend.Interfaces.Entities;

namespace Backend.Entities.Abstract;

public abstract class BaseAuditableEntity : BaseEntity, IAuditable
{
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }

  public int? CreatedById { get; set; }
  public int? UpdatedById { get; set; }

  public User? CreatedBy { get; set; }
  public User? UpdatedBy { get; set; }
}
