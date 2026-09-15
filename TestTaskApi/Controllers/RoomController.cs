using Microsoft.AspNetCore.Mvc;
using TestTaskApi.Application.DTOs;
using TestTaskApi.Application.Services;

namespace TestTaskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        /// <summary>
        /// Отримати список усіх конференц-залів.
        /// </summary>
        /// <returns>Список залів з інформацією про місткість, ціну та послуги.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(rooms);
        }

        /// <summary>
        /// Створити новий конференц-зал.
        /// </summary>
        /// <param name="createRoomDto">Назва, місткість, базова ціна за годину та список Id послуг.</param>
        /// <returns>Створений зал з присвоєним Id.</returns>
        [HttpPost]
        public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto createRoomDto)
        {
            var room = await _roomService.CreateRoomAsync(createRoomDto);

            // 201 Created + ссылка на GetById (ниже) + сам созданный объект
            return CreatedAtAction(nameof(GetAll), new { id = room.Id }, room);
        }

        /// <summary>
        /// Оновити інформацію про зал (частково — можна передати лише ті поля, які потрібно змінити).
        /// </summary>
        /// <param name="id">Id залу.</param>
        /// <param name="updateRoomDto">Поля для оновлення (null означає "не змінювати").</param>
        [HttpPut("{id}")]
        public async Task<ActionResult<RoomDto>> Update(int id, [FromBody] UpdateRoomDto updateRoomDto)
        {
            var room = await _roomService.UpdateRoomAsync(id, updateRoomDto);

            if (room is null)
                return NotFound($"Зал з Id={id} не знайдено.");

            return Ok(room);
        }

        /// <summary>
        /// Видалити конференц-зал за Id.
        /// </summary>
        /// <param name="id">Id залу.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _roomService.DeleteRoomAsync(id);

            if (!success)
                return NotFound($"Зал з Id={id} не знайдено.");

            return NoContent(); // 204
        }

        /// <summary>
        /// Додати послугу до залу.
        /// </summary>
        [HttpPost("{roomId}/utilities/{utilityId}")]
        public async Task<IActionResult> AddUtility(int roomId, int utilityId)
        {
            var success = await _roomService.AddUtilityToRoomAsync(roomId, utilityId);

            if (!success)
                return NotFound("Зал або послуга не знайдена.");

            return NoContent();
        }

        /// <summary>
        /// Прибрати послугу із залу.
        /// </summary>
        [HttpDelete("{roomId}/utilities/{utilityId}")]
        public async Task<IActionResult> RemoveUtility(int roomId, int utilityId)
        {
            var success = await _roomService.RemoveUtilityFromRoomAsync(roomId, utilityId);

            if (!success)
                return NotFound($"Зал з Id={roomId} не знайдено.");

            return NoContent();
        }

        /// <summary>
        /// Пошук вільних залів на заданий інтервал часу з опціональним фільтром за місткістю.
        /// </summary>
        /// <param name="start">Початок бажаного інтервалу.</param>
        /// <param name="end">Кінець бажаного інтервалу.</param>
        /// <param name="capacity">Мінімальна необхідна місткість (необов'язково).</param>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailable(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        [FromQuery] int? capacity)
        {
            if (start >= end)
                return BadRequest("Дата початку має бути раніше дати закінчення.");

            start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

            var rooms = await _roomService.GetAvailableRoomsAsync(start, end, capacity);
            return Ok(rooms);
        }
    }
}