namespace Backend.DTOs;

public class RoasterRevisionSeedDto : RoasterSeedDto
{
  public int RoasterId { get; set; }
  public int EntityRevisionId { get; set; }
}