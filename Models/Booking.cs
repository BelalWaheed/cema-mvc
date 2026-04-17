using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cema.Models
{
    [Table("Booking")] // 2. Force SQL Server to name the table 
    public class Booking
    {

        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ScreeningId { get; set; }

        public DateTime BookingDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(ScreeningId))]
        public Screening? Screening { get; set; }

        public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}