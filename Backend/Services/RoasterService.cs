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

  public async Task<ServiceResult<RoasterDto>> GetRoasterAsync(int id)
  {
    var roaster = await roasterRepository.GetRoasterByIdAsync(id);
    if (roaster == null) return ServiceResult<RoasterDto>.Failure(404, $"Roaster with ID {id} not found");

    return ServiceResult<RoasterDto>.Success(200, roaster);
  }

  public Task<PagedList<RoasterDto>> GetFilteredRoastersAsync()
  {
    throw new NotImplementedException();
  }

  public Task<ServiceResult<RoasterDto>> CreateRoasterAsync()
  {
    throw new NotImplementedException();
  }

  public Task<ServiceResult<RoasterDto>> UpdateRoasterAsync()
  {
    throw new NotImplementedException();
  }

  public Task<ServiceResult<bool>> DeleteRoasterAsync()
  {
    throw new NotImplementedException();
  }
}
