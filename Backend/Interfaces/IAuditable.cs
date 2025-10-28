namespace Backend.Entities;

public interface IAuditable
{
  DateTime CreatedAt { get; set; }
  DateTime UpdatedAt { get; set; }

  int? CreatedById { get; set; }
  int? UpdatedById { get; set; }

  User? CreatedBy { get; set; }
  User? UpdatedBy { get; set; }
}