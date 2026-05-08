using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Required for IFormFile to upload image
namespace CemaApp.ViewModels
{
    public class MovieVM
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
        public int DurationMinutes { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        // IFormFile handles data of the uploaded image
        public IFormFile? PosterImage { get; set; }
    }
}
