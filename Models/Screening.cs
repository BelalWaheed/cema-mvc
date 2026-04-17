using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cema.Models
{
    [Table("Screening")] // 2. Force SQL Server to name the table 

    public class Screening
    {
        [Key]
        public int Id { get; set; }

        public int MovieId { get; set; }
        public int Hall_Id { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [MaxLength(50)]
        public string? Subtitle { get; set; }

        // Navigation properties
        [ForeignKey(nameof(MovieId))]
        public Movie? Movie { get; set; }

        [ForeignKey(nameof(Hall_Id))]
        public Hall? Hall { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<SeatLock> SeatLocks { get; set; } = new List<SeatLock>();
    }
}