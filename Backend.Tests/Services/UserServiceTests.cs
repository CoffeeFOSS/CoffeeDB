using Backend.Common;
using Backend.Common.Params;
using Backend.DTOs;
using Backend.Tests.Services.Fixtures;
using Moq;

namespace Backend.Tests.Services;

[Collection("UserService Collection")]
public class UserServiceTests(UserServiceTestFixture fixture)
{
  #region GetUserAsync

  [Fact]
  public async Task GetUserAsync_UserExists_ReturnsSuccess()
  {
    var username = "robchen";
    var memberDto = new MemberDto { Id = 0, Username = username };
    fixture.MockRepo.Setup(r => r.GetMemberAsync(username)).ReturnsAsync(memberDto);

    var result = await fixture.UserService.GetUserAsync(username);

    Assert.True(result.IsSuccess);
    Assert.Equal(200, result.StatusCode);
    Assert.Equal(username, result.Data?.Username);
  }

  [Fact]
  public async Task GetUserAsync_UserDoesNotExist_ReturnsFailure()
  {
    var username = "robchen";
    fixture.MockRepo.Setup(r => r.GetMemberAsync(username)).ReturnsAsync((MemberDto?)null);

    var result = await fixture.UserService.GetUserAsync(username);

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
    fixture.MockRepo.Setup(r => r.GetMembersAsync(It.IsAny<UserParams>())).ReturnsAsync(pagedList);

    var result = await fixture.UserService.GetUsersAsync(userParams, fixture.HttpContext.Response);

    Assert.Equal(2, result.Count);
    Assert.Contains(result, u => u.Username == "bart");
    Assert.Contains(result, u => u.Username == "abarn");
    Assert.True(fixture.HttpContext.Response.Headers.ContainsKey("Pagination"));
  }

  #endregion
}
