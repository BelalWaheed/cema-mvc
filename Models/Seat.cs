using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cema.Models
{
    [Table("Seat")] // 2. Force SQL Server to name the table 

    public class Seat
    {
        [Key]
        public int Id { get; set; }

        public int Hall_Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Row { get; set; } = string.Empty;

        public int Number { get; set; }

        [Required]
        [MaxLength(50)]
        public string SeatType { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey(nameof(Hall_Id))]
        public Hall? Hall { get; set; }

        public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
        public ICollection<SeatLock> SeatLocks { get; set; } = new List<SeatLock>();
    }
}