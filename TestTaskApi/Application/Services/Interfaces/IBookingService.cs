using TestTaskApi.Application.DTOs;

namespace TestTaskApi.Application.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto);
    }
}
