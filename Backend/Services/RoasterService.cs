using Backend.Common;
using Backend.Common.Params;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Extensions;
using Backend.Interfaces.Repository;
using Backend.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class RoasterService(IRoasterRepository roasterRepository) : IRoasterService
{
  public async Task<PagedList<RoasterDto>> GetRoastersAsync(RoasterParams roasterParams, HttpResponse response)
  {
    var roasters = await roasterRepository.GetRoastersAsync(roasterParams);
    response.AddPaginationHeader(roasters);

    return roasters;
  }

  public async Task<ServiceResult<RoasterDto>> GetRoasterAsync(int id)
  {
    var roaster = await roasterRepository.GetRoasterByIdAsync(id);

    if (roaster == null)
      return ServiceResult<RoasterDto>.Failure(404, $"Roaster with ID {id} not found");

    return ServiceResult<RoasterDto>.Success(200, roaster);
  }

  public async Task<ServiceResult<RoasterDto>> CreateRoasterAsync(CreateRoasterDto createRoasterDto)
  {
    if (string.IsNullOrWhiteSpace(createRoasterDto.Name))
      return ServiceResult<RoasterDto>.Failure(400, "Name must be provided");

    if (await roasterRepository.RoasterExistsAsync(createRoasterDto.Name, createRoasterDto.Location))
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster '{createRoasterDto.Name}' already exists in location '{createRoasterDto.Location}'");

    if (!string.IsNullOrWhiteSpace(createRoasterDto.Name) && !Uri.TryCreate(createRoasterDto.WebsiteUrl, UriKind.Absolute, out _))
      return ServiceResult<RoasterDto>.Failure(400, $"'{createRoasterDto.WebsiteUrl}' is not a valid URL'");

    var roaster = await roasterRepository.CreateRoasterAsync(createRoasterDto);

    if (roaster == null)
      return ServiceResult<RoasterDto>.Failure(500, "Could not create roaster");

    return ServiceResult<RoasterDto>.Success(200, roaster);
  }

  public async Task<ServiceResult<RoasterDto>> UpdateRoasterAsync()
  {
    throw new NotImplementedException();
  }

  public async Task<ServiceResult<bool>> DeleteRoasterAsync()
  {
    throw new NotImplementedException();
  }
}
