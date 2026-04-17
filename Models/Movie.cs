using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cema.Models
{
    [Table("Movie")] // 2. Force SQL Server to name the table 

    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Genre { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        [MaxLength(50)]
        public string? Language { get; set; }

        public DateTime ReleaseDate { get; set; }

        [MaxLength(10)]
        public string? Rating { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Poster { get; set; }
        public string? Trailer { get; set; }

        // Navigation properties
        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
    }
}