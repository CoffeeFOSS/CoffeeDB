namespace Backend.DTOs;

public class RoasterDto
{
  public required int Id { get; set; }
  public required string Name { get; set; }
  public string? Alias { get; set; }
  public string? LocationAddress { get; set; }
  public CoordinatesDto? LocationCoordinates { get; set; }
  public string? WebsiteUrl { get; set; }
  public string? Description { get; set; }
  public int? DistanceInMeters { get; set; }
}
