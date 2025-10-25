namespace Backend.Entities;

public abstract class BaseAuditableEntity : BaseEntity, IAuditable
{
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public int? CreatedById { get; set; }
  public int? UpdatedById { get; set; }
  public User? CreatedBy { get; set; } = null!;
  public User? UpdatedBy { get; set; } = null!;
}
