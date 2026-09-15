using System.ComponentModel.DataAnnotations;

namespace TestTaskApi.Application.DTOs
{
    public class UpdateRoomDto
    {
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Назва має бути від 1 до 100 символів.")]
        public string? Name { get; set; }

        [Range(1, 1000, ErrorMessage = "Місткість має бути від 1 до 1000 осіб.")]
        public int? Capacity { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "Базова ціна має бути додатною.")]
        public decimal? BasePricePerHour { get; set; }
    }
}