using System.ComponentModel.DataAnnotations;

namespace TestTaskApi.Application.DTOs
{
    public class CreateBookingDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Вкажіть коректний Id залу.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Дата початку обов'язкова.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Дата закінчення обов'язкова.")]
        public DateTime EndDate { get; set; }

        public List<int> UtilityIds { get; set; } = new();
    }
}