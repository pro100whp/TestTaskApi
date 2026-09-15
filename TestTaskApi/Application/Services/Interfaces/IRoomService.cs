using TestTaskApi.Application.DTOs;

namespace TestTaskApi.Application.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
        Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto);
        Task<RoomDto?> UpdateRoomAsync(int id, UpdateRoomDto updateRoomDto);
        Task<bool> DeleteRoomAsync(int id);
        Task<bool> AddUtilityToRoomAsync(int roomId, int utilityId);
        Task<bool> RemoveUtilityFromRoomAsync(int roomId, int utilityId);
        Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(DateTime start, DateTime end, int? capacity);

    }
}
