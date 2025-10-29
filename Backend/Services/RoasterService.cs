using Backend.Common;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Interfaces;

namespace Backend.Services;

public class RoasterService(IRoasterRepository roasterRepository) : IRoasterService
{
  public async Task<PagedList<RoasterDto>> GetRoastersAsync(UserParams userParams, HttpResponse response)
  {
    var roasters = await roasterRepository.GetRoastersAsync(userParams);
    response.AddPaginationHeader(roasters);

    return roasters;
  }
}
