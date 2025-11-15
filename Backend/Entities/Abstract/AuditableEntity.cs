using Backend.Interfaces.Entities;

namespace Backend.Entities.Abstract;

public abstract class AuditableEntity : IAuditable
{
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  public int? CreatedById { get; set; }
  public int? UpdatedById { get; set; }

  public User? CreatedBy { get; set; } = null!;
  public User? UpdatedBy { get; set; } = null!;
}
