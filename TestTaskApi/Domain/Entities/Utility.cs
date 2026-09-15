namespace TestTaskApi.Domain.Entities
{
    public class Utility
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public List<Room> Rooms { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
            

    }
}
