using Backend.Common;
using Backend.DTOs;

namespace Backend.Interfaces;

public interface IRoasterService
{
  Task<PagedList<RoasterDto>> GetRoastersAsync(UserParams userParams, HttpResponse response);
  // Task<RoasterDto?> GetRoasterAsync(int id);
}