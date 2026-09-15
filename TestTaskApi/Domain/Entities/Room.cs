namespace TestTaskApi.Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal BasePricePerHour { get; set; }
        public List<Utility> Utilities { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();


    }
}
