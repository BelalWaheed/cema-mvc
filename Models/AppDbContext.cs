using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CemaApp.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<SeatLock> SeatLocks { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Movie -> Screenings
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Screenings)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Hall -> Screenings
            modelBuilder.Entity<Hall>()
                .HasMany(h => h.Screenings)
                .WithOne(s => s.Hall)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Restrict);

            // Hall -> Seats
            modelBuilder.Entity<Hall>()
                .HasMany(h => h.Seats)
                .WithOne(s => s.Hall)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Cascade);

            // Screening -> Bookings
            modelBuilder.Entity<Screening>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.Screening)
                .HasForeignKey(b => b.ScreeningId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Bookings
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking -> BookingSeats
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingSeats)
                .WithOne(bs => bs.Booking)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seat -> BookingSeats
            modelBuilder.Entity<Seat>()
                .HasMany(s => s.BookingSeats)
                .WithOne(bs => bs.Seat)
                .HasForeignKey(bs => bs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Screening -> SeatLocks
            modelBuilder.Entity<Screening>()
                .HasMany(s => s.SeatLocks)
                .WithOne(sl => sl.Screening)
                .HasForeignKey(sl => sl.ScreeningId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Seat -> SeatLocks
            modelBuilder.Entity<Seat>()
                .HasMany(s => s.SeatLocks)
                .WithOne(sl => sl.Seat)
                .HasForeignKey(sl => sl.SeatId)
                .OnDelete(DeleteBehavior.Cascade);

            //  User -> SeatLocks
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.SeatLocks)
                .WithOne(sl => sl.User)
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: Can't have duplicate seat in same hall
            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.HallId, s.Row, s.Number })
                .IsUnique();

            //  Index for cleaning expired locks quickly
            modelBuilder.Entity<SeatLock>()
                .HasIndex(sl => sl.ExpiresAt);

            // EF requires explicit configuration for decimal properties
            modelBuilder.Entity<Screening>()
            .Property(s => s.Price)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");
        }
    }
}
