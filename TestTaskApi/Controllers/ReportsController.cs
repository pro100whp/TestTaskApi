using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestTaskApi.Application.DTOs;
using TestTaskApi.Application.Services;
using TestTaskApi.Application.Services.Interfaces;

namespace TestTaskApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Звіт про виручку по кожному залу за весь час.
        /// </summary>
        [HttpGet("revenue-by-room")]
        public async Task<ActionResult<IEnumerable<RevenueByRoomDto>>> GetRevenueByRoom()
        {
            var result = await _reportService.GetRevenueByRoomAsync();
            return Ok(result);
        }

        /// <summary>
        /// Найпопулярніший зал за кількістю бронювань.
        /// </summary>
        [HttpGet("most-popular-room")]
        public async Task<ActionResult<PopularRoomDto>> GetMostPopularRoom()
        {
            var result = await _reportService.GetMostPopularRoomAsync();

            if (result is null)
                return Ok(new { message = "Поки що немає жодного бронювання." });

            return Ok(result);
        }

        /// <summary>
        /// Середня вартість одного бронювання по всій системі.
        /// </summary>
        [HttpGet("average-booking-price")]
        public async Task<ActionResult<AverageBookingPriceDto>> GetAverageBookingPrice()
        {
            var result = await _reportService.GetAverageBookingPriceAsync();
            return Ok(result);
        }
    }
}