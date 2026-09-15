using System.Numerics;

namespace TestTaskApi.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public List<Utility> Utilities { get; set; } = new();
        public List<Room> Rooms { get; set; } = new();

    }
}
