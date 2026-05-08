using System.ComponentModel.DataAnnotations.Schema;

namespace CemaApp.Models
{
    [Table("SeatLock")]

    public class SeatLock
    {
        public int Id { get; set; }

        public int ScreeningId { get; set; }

        public int SeatId { get; set; }

        public string UserId { get; set; }

        public DateTime ExpiresAt { get; set; } // Lock expires after 5 minutes

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Screening Screening { get; set; }
        public Seat Seat { get; set; }
        public ApplicationUser User { get; set; }

    }
}
