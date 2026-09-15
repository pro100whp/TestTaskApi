namespace TestTaskApi.Application.DTOs
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public decimal BasePricePerHour { get; set; }
        public List<UtilityDto> Utilities { get; set; } = new();
    }
}
