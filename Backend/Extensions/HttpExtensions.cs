using System.Text.Json;
using Backend.Common;

namespace Backend.Extensions;

public static class HttpExtensions
{
  private static readonly JsonSerializerOptions jsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
  {
    var paginationHeader = new PaginationHeader(
      data.CurrentPage,
      data.PageSize,
      data.TotalCount,
      data.TotalPages
    );

    response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));
    response.Headers.Append("Access-Control-Expose-Headers", "Pagination"); // Expose "Pagination" header to client

  }
}