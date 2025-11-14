using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Backend.Tests.Services;

public class UserServiceTests
{
  private readonly Mock<IUserRepository> _mockRepo = new();
  private readonly DefaultHttpContext _httpContext = new();
  private readonly IUserService _userService;

  public UserServiceTests()
  {
    _userService = new UserService(_mockRepo.Object);
  }

  #region GetUserAsync

  [Fact]
  public async Task GetUserAsync_UserExists_ReturnsSuccess()
  {
    var username = "robchen";
    var memberDto = new MemberDto { Id = 0, Username = username };
    _mockRepo.Setup(r => r.GetMemberAsync(username)).ReturnsAsync(memberDto);

    var result = await _userService.GetUserAsync(username);

    Assert.True(result.IsSuccess);
    Assert.Equal(200, result.StatusCode);
    Assert.Equal(username, result.Data?.Username);
  }

  [Fact]
  public async Task GetUserAsync_UserDoesNotExist_ReturnsFailure()
  {
    var username = "robchen";
    _mockRepo.Setup(r => r.GetMemberAsync(username)).ReturnsAsync((MemberDto?)null);

    var result = await _userService.GetUserAsync(username);

    Assert.False(result.IsSuccess);
    Assert.Equal(404, result.StatusCode);
    Assert.Null(result.Data?.Username);
  }

  #endregion

  #region GetUsersAsync

  [Fact]
  public async Task GetUsersAsync_AddsPaginationHeaderAndReturnsPagedList()
  {
    var userParams = new UserParams { Username = "bar" };
    var users = new List<MemberDto>
    {
      new() { Id = 1, Username = "bart" },
      new() { Id = 1, Username = "abarn" },
    };
    var pagedList = new PagedList<MemberDto>(users, users.Count, page: 1, pageSize: 10);
    _mockRepo.Setup(r => r.GetMembersAsync(It.IsAny<UserParams>())).ReturnsAsync(pagedList);

    var result = await _userService.GetUsersAsync(userParams, _httpContext.Response);

    Assert.Equal(2, result.Count);
    Assert.Contains(result, u => u.Username == "bart");
    Assert.Contains(result, u => u.Username == "abarn");
    Assert.True(_httpContext.Response.Headers.ContainsKey("Pagination"));
  }

  #endregion
}
