using System.Security.Claims;

namespace Backend.Common;

public class UserClaimsUtils
{
  public static int GetUserId(ClaimsPrincipal userClaims)
  {
    var userIdString = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userIdString))
      throw new InvalidOperationException("User ID not found in claims");

    return int.Parse(userIdString);
  }
}
