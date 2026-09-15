namespace TestTaskApi.Application.DTOs
{
    public class RevenueByRoomDto
    {
        public string RoomName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int BookingsCount { get; set; }
    }
}