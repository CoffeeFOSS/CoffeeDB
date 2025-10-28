using Backend.Common;
using Backend.DTOs;
using Backend.Interfaces;

namespace Backend.Services;

public class RoasterService : IRoasterService
{
  public async Task<PagedList<RoasterDto>> GetRoastersAsync(UserParams userParams, HttpResponse response)
  {
    throw new NotImplementedException();
  }
}
