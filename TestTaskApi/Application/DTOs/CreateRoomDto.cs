using System.ComponentModel.DataAnnotations;

namespace TestTaskApi.Application.DTOs
{
    public class CreateRoomDto
    {
        [Required(ErrorMessage = "Назва залу обов'язкова.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Назва повинна бути від 1 до 100 символів.")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 1000, ErrorMessage = "Місткість повинна бути від 1 до 100 людей.")]
        public int Capacity { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "Базова ціна повинна бути більше 0.")]
        public decimal BasePricePerHour { get; set; }

        public List<int> UtilityIds { get; set; } = new();
    }
}