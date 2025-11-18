namespace Backend.DTOs;

public class RoasterRevisionSeedDto : RoasterSeedDto
{
  public int RoasterId { get; set; }
  public int RevisionMetadataId { get; set; }
}