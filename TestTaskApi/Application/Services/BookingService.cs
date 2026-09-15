using Microsoft.EntityFrameworkCore;
using TestTaskApi.Application.DTOs;
using TestTaskApi.Application.Services.Interfaces;
using TestTaskApi.Domain.Entities;
using TestTaskApi.Infrastructure.Data;

namespace TestTaskApi.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPricingService _pricingService;

        public BookingService(ApplicationDbContext context, IPricingService pricingService)
        {
            _context = context;
            _pricingService = pricingService;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Utilities)
                .ToListAsync();

            return bookings.Select(MapToDto);
        }

        public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto)
        {
            var room = await _context.Rooms.FindAsync(dto.RoomId);

            if (room is null)
                return null; // Контроллер вернёт 404

            var isOccupied = await _context.Bookings.AnyAsync(b =>
                b.RoomId == dto.RoomId &&
                b.StartDate < dto.EndDate &&
                b.EndDate > dto.StartDate);

            if (isOccupied)
                throw new InvalidOperationException("Зал вже зайнятий.");

            var utilities = await _context.Utilities
                .Where(u => dto.UtilityIds.Contains(u.Id))
                .ToListAsync();

            var roomCost = _pricingService.CalculateRoomCost(dto.StartDate, dto.EndDate, room.BasePricePerHour);
            var utilitiesCost = utilities.Sum(u => u.BasePrice);

            var booking = new Booking
            {
                RoomId = room.Id,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalPrice = Math.Round(roomCost + utilitiesCost, 2),
                Utilities = utilities
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            booking.Room = room;
            return MapToDto(booking);
        }

        private static BookingDto MapToDto(Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                RoomName = booking.Room?.Name ?? string.Empty,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                TotalPrice = booking.TotalPrice,
                Utilities = booking.Utilities.Select(u => new UtilityDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    BasePrice = u.BasePrice
                }).ToList()
            };
        }
    }
}
