namespace Backend.Common.Params;

public class RevisionParams : PaginationParams
{
  public bool? CommittedOnly { get; set; }
}
