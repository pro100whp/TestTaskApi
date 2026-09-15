using TestTaskApi.Application.DTOs;

namespace TestTaskApi.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<RevenueByRoomDto>> GetRevenueByRoomAsync();
        Task<PopularRoomDto?> GetMostPopularRoomAsync();
        Task<AverageBookingPriceDto> GetAverageBookingPriceAsync();
    }
}
