using Microsoft.EntityFrameworkCore;
using TestTaskApi.Domain.Entities;

namespace TestTaskApi.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Utility> Utilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Room 1 - * Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId);

            // Room * - * Utility
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Utilities)
                .WithMany(u => u.Rooms);

            // Booking * - * Utility
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Utilities)
                .WithMany(u => u.Bookings);

            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "Room A", Capacity = 50, BasePricePerHour = 2000 },
                new Room { Id = 2, Name = "Room B", Capacity = 100, BasePricePerHour = 3500 },
                new Room { Id = 3, Name = "Room C", Capacity = 30, BasePricePerHour = 1500 }
            );
            modelBuilder.Entity<Utility>().HasData(
                new Utility { Id = 1, Name = "Projector", BasePrice = 500 },
                new Utility { Id = 2, Name = "Wi-Fi", BasePrice = 300 },
                new Utility { Id = 3, Name = "Sound", BasePrice = 700 }
            );
        }
    }
}