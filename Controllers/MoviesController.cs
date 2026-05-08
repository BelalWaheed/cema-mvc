using CemaApp.Models;
using CemaApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CemaApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MoviesController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.AsNoTracking().ToListAsync();
            return View(movies);
        }

        // GET: Movies/Create
        // This simply returns the empty form to the Admin
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieVM model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                //  Handle the Image Upload if a file was provided
                if (model.PosterImage != null)
                {
                    // Define where to save the image (wwwroot/images/posters)
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "posters");

                    Directory.CreateDirectory(uploadsFolder);

                    // Ensure the file name is unique 
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PosterImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Copy the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PosterImage.CopyToAsync(fileStream);
                    }
                }

                Movie newMovie = new Movie
                {
                    Title = model.Title,
                    Description = model.Description,
                    Genre = model.Genre,
                    DurationMinutes = model.DurationMinutes,
                    ReleaseDate = model.ReleaseDate,
                    PosterUrl = uniqueFileName,
                    IsActive = true
                };

                _context.Movies.Add(newMovie);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}