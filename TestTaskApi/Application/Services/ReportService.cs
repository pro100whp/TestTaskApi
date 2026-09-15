using Microsoft.EntityFrameworkCore;
using TestTaskApi.Application.DTOs;
using TestTaskApi.Application.Services.Interfaces;
using TestTaskApi.Infrastructure.Data;

namespace TestTaskApi.Application.Services
{
    

    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RevenueByRoomDto>> GetRevenueByRoomAsync()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .GroupBy(b => b.Room.Name)
                .Select(g => new RevenueByRoomDto
                {
                    RoomName = g.Key,
                    TotalRevenue = g.Sum(b => b.TotalPrice),
                    BookingsCount = g.Count()
                })
                .OrderByDescending(r => r.TotalRevenue)
                .ToListAsync();
        }

        public async Task<PopularRoomDto?> GetMostPopularRoomAsync()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .GroupBy(b => b.Room.Name)
                .Select(g => new PopularRoomDto
                {
                    RoomName = g.Key,
                    BookingsCount = g.Count()
                })
                .OrderByDescending(r => r.BookingsCount)
                .FirstOrDefaultAsync();
        }

        public async Task<AverageBookingPriceDto> GetAverageBookingPriceAsync()
        {
            var hasBookings = await _context.Bookings.AnyAsync();

            if (!hasBookings)
                return new AverageBookingPriceDto { AveragePrice = 0m };

            var average = await _context.Bookings.AverageAsync(b => b.TotalPrice);
            return new AverageBookingPriceDto { AveragePrice = Math.Round(average, 2) };
        }
    }
}