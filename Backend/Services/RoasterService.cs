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
    {
      if (createRoasterDto.Location == null)
        return ServiceResult<RoasterDto>.Failure(400, $"Roaster '{createRoasterDto.Name}' already exists without a specified location");
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster '{createRoasterDto.Name}' already exists in location '{createRoasterDto.Location}'");
    }

    if (!string.IsNullOrWhiteSpace(createRoasterDto.WebsiteUrl) && !UrlValidator.IsValidUrl(createRoasterDto.WebsiteUrl))
      return ServiceResult<RoasterDto>.Failure(400, $"'{createRoasterDto.WebsiteUrl}' is not a valid URL'");

    var roaster = await roasterRepository.CreateRoasterAsync(createRoasterDto);

    if (roaster == null)
      return ServiceResult<RoasterDto>.Failure(500, "Could not create roaster");

    return ServiceResult<RoasterDto>.Success(200, roaster);
  }

  public async Task<ServiceResult<RoasterDto>> UpdateRoasterAsync(int id, UpdateRoasterDto updateRoasterDto)
  {
    var currentRoaster = await roasterRepository.GetRoasterByIdAsync(id);

    if (currentRoaster == null)
      return ServiceResult<RoasterDto>.Failure(400, $"Roaster ID '{id}' does not exist");

    if (!string.IsNullOrWhiteSpace(updateRoasterDto.Name))
    {
      if (await roasterRepository.RoasterExistsAsync(updateRoasterDto.Name, updateRoasterDto.Location, id))
      {
        string locationInfo = updateRoasterDto.Location == null ?
            "without a specified location" : $"in location '{updateRoasterDto.Location}'";
        return ServiceResult<RoasterDto>.Failure(400,
            $"Roaster '{updateRoasterDto.Name}' already exists {locationInfo}.");
      }
    }

    if (!string.IsNullOrWhiteSpace(updateRoasterDto.WebsiteUrl) && !UrlValidator.IsValidUrl(updateRoasterDto.WebsiteUrl))
      return ServiceResult<RoasterDto>.Failure(400, $"'{updateRoasterDto.WebsiteUrl}' is not a valid URL'");

    // check if at least one field is different from curr
    var updatedRoaster = await roasterRepository.UpdateRoasterAsync(id, updateRoasterDto);

    if (updatedRoaster == null)
      return ServiceResult<RoasterDto>.Failure(500, "Could not update roaster (no changes in DTO or DB error)");

    return ServiceResult<RoasterDto>.Success(200, updatedRoaster);
  }

  public async Task<ServiceResult<object>> DeleteRoasterAsync(int id)
  {
    if (!await roasterRepository.RoasterExistsByIdAsync(id))
      return ServiceResult<object>.Failure(400, $"Roaster ID '{id}' does not exist");

    var result = await roasterRepository.DeleteRoasterAsync(id);

    if (!result)
      return ServiceResult<object>.Failure(500, "Could not delete roaster (other entities reference roaster)");

    return ServiceResult<object>.Success(200, null);
  }
}
