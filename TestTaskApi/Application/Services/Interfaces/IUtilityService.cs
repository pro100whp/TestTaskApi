using TestTaskApi.Application.DTOs;

namespace TestTaskApi.Application.Services.Interfaces
{
    public interface IUtilityService
    {
        Task<IEnumerable<UtilityDto>> GetAllUtilitiesAsync();
        Task<UtilityDto> CreateUtilityAsync(CreateUtilityDto dto);
        Task<UtilityDto?> UpdateUtilityAsync(int id, UpdateUtilityDto dto);
        Task<bool> DeleteUtilityAsync(int id);
    }
}
