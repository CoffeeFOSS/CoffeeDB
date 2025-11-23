using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Backend.Tests.Services.Fixtures;

[CollectionDefinition("UserService Collection")]
public class UserServiceCollection : ICollectionFixture<UserServiceTestFixture> { }

public class UserServiceTestFixture
{
  public Mock<IUserRepository> MockRepo { get; } = new();
  public DefaultHttpContext HttpContext { get; } = new();
  public IUserService UserService { get; }

  public UserServiceTestFixture()
  {
    UserService = new UserService(MockRepo.Object);
  }
}

