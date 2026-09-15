using Microsoft.EntityFrameworkCore;
using TestTaskApi.Application.DTOs;
using TestTaskApi.Infrastructure.Data;
using TestTaskApi.Domain.Entities;

namespace TestTaskApi.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Utilities)
                .ToListAsync();

            return rooms.Select(MapToDto);
        }

        public async Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto)
        {
            var utilities = await _context.Utilities
                .Where(u => createRoomDto.UtilityIds.Contains(u.Id))
                .ToListAsync();

            if (utilities.Count != createRoomDto.UtilityIds.Count)
            {
                throw new InvalidOperationException(
                    "Одна или несколько выбранных услуг не существуют.");
            }

            var room = new Room
            {
                Name = createRoomDto.Name,
                Capacity = createRoomDto.Capacity,
                BasePricePerHour = createRoomDto.BasePricePerHour,
                Utilities = utilities
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return MapToDto(room);
        }

        public async Task<RoomDto?> UpdateRoomAsync(int id, UpdateRoomDto updateRoomDto)
        {
            var room = await _context.Rooms
                .Include(r => r.Utilities)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room is null)
                return null;

            // Обновляем только те поля, которые клиент реально прислал
            if (updateRoomDto.Name is not null)
                room.Name = updateRoomDto.Name;

            if (updateRoomDto.Capacity.HasValue)
                room.Capacity = updateRoomDto.Capacity.Value;

            if (updateRoomDto.BasePricePerHour.HasValue)
                room.BasePricePerHour = updateRoomDto.BasePricePerHour.Value;

            await _context.SaveChangesAsync();

            return MapToDto(room);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room is null)
                return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddUtilityToRoomAsync(int roomId, int utilityId)
        {
            var room = await _context.Rooms
                .Include(r => r.Utilities)
                .FirstOrDefaultAsync(r => r.Id == roomId);
            var utility = await _context.Utilities.FindAsync(utilityId);

            if (room is null || utility is null)
                return false;

            if (!room.Utilities.Any(u => u.Id == utilityId))
                room.Utilities.Add(utility);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveUtilityFromRoomAsync(int roomId, int utilityId)
        {
            var room = await _context.Rooms
                .Include(r => r.Utilities)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room is null)
                return false;

            room.Utilities.RemoveAll(u => u.Id == utilityId);
            await _context.SaveChangesAsync();
            return true;
        }

        
        private static RoomDto MapToDto(Room room)
        {
            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                BasePricePerHour = room.BasePricePerHour,
                Utilities = room.Utilities.Select(u => new UtilityDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    BasePrice = u.BasePrice
                }).ToList()
            };
        }
        public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(DateTime start, DateTime end, int? capacity)
        {
            var query = _context.Rooms
                .Include(r => r.Utilities)
                .Include(r => r.Bookings)
                .Where(r => !r.Bookings.Any(b => b.StartDate < end && b.EndDate > start));

            if (capacity.HasValue)
                query = query.Where(r => r.Capacity >= capacity.Value);

            var rooms = await query.ToListAsync();
            return rooms.Select(MapToDto);
        }
    }
}