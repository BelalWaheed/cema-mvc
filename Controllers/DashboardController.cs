using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CemaApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

        public DashboardController(AppDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new CemaApp.ViewModels.DashboardViewModel();

            // 1. Stats
            viewModel.TotalRevenue = await _context.Bookings
                .Where(b => b.Status == BookingStatus.Confirmed)
                .SumAsync(b => b.TotalPrice);

            viewModel.TotalBookings = await _context.Bookings
                .CountAsync(b => b.Status == BookingStatus.Confirmed);

            viewModel.TotalUsers = await _userManager.Users.CountAsync();

            // 2. Most Popular Movie
            var popularMovieId = await _context.Bookings
                .Where(b => b.Status == BookingStatus.Confirmed)
                .GroupBy(b => b.Screening.MovieId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            if (popularMovieId != 0)
            {
                viewModel.MostPopularMovie = await _context.Movies.FindAsync(popularMovieId);
                viewModel.PopularMovieBookingCount = await _context.Bookings
                    .CountAsync(b => b.Screening.MovieId == popularMovieId && b.Status == BookingStatus.Confirmed);
            }

            // 3. Average Occupancy Rate
            // (Total booked seats / Total capacity across all screenings)
            var totalBookedSeats = await _context.BookingSeats
                .CountAsync(bs => bs.Booking.Status == BookingStatus.Confirmed);

            var screenings = await _context.Screenings.Include(s => s.Hall).ToListAsync();
            var totalCapacity = screenings.Sum(s => s.Hall.TotalRows * s.Hall.SeatsPerRow);

            viewModel.AverageOccupancyRate = totalCapacity > 0 
                ? (double)totalBookedSeats / totalCapacity * 100 
                : 0;

            // 4. Recent Bookings
            viewModel.RecentBookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .Select(b => new CemaApp.ViewModels.RecentBookingViewModel
                {
                    BookingId = b.Id,
                    UserEmail = b.User.Email,
                    MovieTitle = b.Screening.Movie.Title,
                    BookingDate = b.BookingDate,
                    Amount = b.TotalPrice,
                    Status = b.Status
                })
                .ToListAsync();

            return View(viewModel);
        }
    }
}