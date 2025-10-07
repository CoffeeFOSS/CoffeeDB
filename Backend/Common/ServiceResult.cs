namespace Backend.Common;

public class ServiceResult<T>
{
  public bool IsSuccess { get; private set; }
  public int StatusCode { get; private set; }
  public T? Data { get; private set; }
  public string? Message { get; private set; }

  public static ServiceResult<T> Success(int statusCode, T data) => new()
  {
    IsSuccess = true,
    StatusCode = statusCode,
    Data = data
  };

  public static ServiceResult<T> Failure(int statusCode, string message) => new()
  {
    IsSuccess = false,
    StatusCode = statusCode,
    Message = message
  };
}
