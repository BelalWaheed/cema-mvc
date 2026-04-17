using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cema.Models
{
    [Table("SeatLock")] // 2. Force SQL Server to name the table 

    public class SeatLock
    {
        [Key]
        public int Id { get; set; }

        public int ScreeningId { get; set; }
        public int SeatId { get; set; }
        public int UserId { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ScreeningId))]
        public Screening? Screening { get; set; }

        [ForeignKey(nameof(SeatId))]
        public Seat? Seat { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}