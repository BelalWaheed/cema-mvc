using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cema.Models
{
    [Table("Hall")] // 2. Force SQL Server to name the table 

    public class Hall
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string HallType { get; set; } = string.Empty;

        public int TotalRows { get; set; }
        public int SeatsPerRow { get; set; }

        // Navigation properties
        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}