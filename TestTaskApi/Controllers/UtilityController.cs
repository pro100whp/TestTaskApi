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
    public class UtilityController : ControllerBase
    {
        private readonly IUtilityService _utilityService;

        public UtilityController(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        /// <summary>
        /// Отримати список усіх доступних послуг.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UtilityDto>>> GetAll()
        {
            return Ok(await _utilityService.GetAllUtilitiesAsync());
        }

        /// <summary>
        /// Додати нову послугу до каталогу.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UtilityDto>> Create([FromBody] CreateUtilityDto dto)
        {
            var utility = await _utilityService.CreateUtilityAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = utility.Id }, utility);
        }

        /// <summary>
        /// Оновити назву або ціну існуючої послуги.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UtilityDto>> Update(int id, [FromBody] UpdateUtilityDto dto)
        {
            var utility = await _utilityService.UpdateUtilityAsync(id, dto);
            if (utility is null) return NotFound($"Послуга з Id={id} не знайдена.");
            return Ok(utility);
        }

        /// <summary>
        /// Видалити послугу з каталогу.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _utilityService.DeleteUtilityAsync(id);
            if (!success) return NotFound($"Послуга з Id={id} не знайдена.");
            return NoContent();
        }
    }
}