namespace Backend.DTOs;

public class RevisionMetadataContributionDto : RevisionMetadataDto
{
  public string EntityName { get; set; } = null!;
  public int? EntityId { get; set; } = null!;
}
