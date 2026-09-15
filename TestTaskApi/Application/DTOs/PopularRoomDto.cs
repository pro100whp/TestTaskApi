namespace TestTaskApi.Application.DTOs
{
    public class PopularRoomDto
    {
        public string RoomName { get; set; } = string.Empty;
        public int BookingsCount { get; set; }
    }
}