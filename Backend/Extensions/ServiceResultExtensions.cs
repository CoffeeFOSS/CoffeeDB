using Backend.Common;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Extensions;

public static class ServiceResultExtensions
{
  public static IActionResult ToActionResult<T>(this ServiceResult<T> result)
    => result.IsSuccess
      ? new ObjectResult(result.Data) { StatusCode = result.StatusCode }
      : new ObjectResult(result.Message) { StatusCode = result.StatusCode };
}
