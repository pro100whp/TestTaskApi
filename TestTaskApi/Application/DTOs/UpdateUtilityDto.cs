using System.ComponentModel.DataAnnotations;

namespace TestTaskApi.Application.DTOs
{
    public class UpdateUtilityDto
    {
        [StringLength(100, MinimumLength = 1)]
        public string? Name { get; set; }

        [Range(0.01, 1000000)]
        public decimal? BasePrice { get; set; }
    }
}