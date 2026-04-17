using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cema.Models
{
    [Table("BookingSeat")] // 2. Force SQL Server to name the table 
    public class BookingSeat
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }
        public int SeatId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtBooking { get; set; }

        // Navigation properties
        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        [ForeignKey(nameof(SeatId))]
        public Seat? Seat { get; set; }
    }
}