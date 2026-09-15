using System.ComponentModel.DataAnnotations;

namespace TestTaskApi.Application.DTOs
{
    public class CreateUtilityDto
    {
        [Required(ErrorMessage = "Назва послуги обов'язкова.")]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 1000000, ErrorMessage = "Ціна послуги має бути додатною.")]
        public decimal BasePrice { get; set; }
    }
}