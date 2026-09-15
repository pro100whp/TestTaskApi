using Microsoft.AspNetCore.Mvc;
using TestTaskApi.Application.DTOs;
using TestTaskApi.Application.Services;
using TestTaskApi.Application.Services.Interfaces;

namespace TestTaskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Отримати список усіх бронювань.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        /// <summary>
        /// Забронювати зал на певний інтервал часу з опціональними додатковими послугами.
        /// Автоматично розраховує вартість оренди з урахуванням тарифних годин.
        /// </summary>
        /// <param name="dto">Id залу, дата/час початку та кінця, список Id обраних послуг.</param>
        /// <returns>Підтвердження бронювання з розрахованою загальною вартістю.</returns>
        [HttpPost]
        public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto dto)
        {
            if (dto.StartDate >= dto.EndDate)
                return BadRequest("Дата початку має бути раніше дати закінчення.");

            dto.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
            dto.EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);

            try
            {
                var booking = await _bookingService.CreateBookingAsync(dto);

                if (booking is null)
                    return NotFound($"Зал з Id={dto.RoomId} не знайден.");

                return CreatedAtAction(nameof(GetAll), new { id = booking.Id }, booking);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // 409 — зал занят
            }
        }
    }
}