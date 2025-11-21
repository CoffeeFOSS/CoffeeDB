namespace Backend.DTOs;

public class RevisionMetadataWithEntityIdentifierDto : RevisionMetadataDto
{
  public string EntityName { get; set; } = null!;
  public int? EntityId { get; set; } = null!;
}
