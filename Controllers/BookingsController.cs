using CemaApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CemaApp.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

        public BookingsController(AppDbContext context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings (User's History & Profile)
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null) return NotFound();

            var bookings = await _context.Bookings
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Hall)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .Where(b => b.UserId == userId && b.Status != BookingStatus.Cancelled)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            var viewModel = new CemaApp.ViewModels.UserBookingsViewModel
            {
                User = user,
                Bookings = bookings
            };

            return View(viewModel);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.Bookings
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Screening)
                    .ThenInclude(s => s.Hall)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (booking == null) return NotFound();

            return View(booking);
        }
        // POST: Bookings/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null) return NotFound();

            // Only allow cancellation if status is not already Cancelled
            if (booking.Status != BookingStatus.Cancelled)
            {
                booking.Status = BookingStatus.Cancelled;
                await _context.SaveChangesAsync();
                TempData["Message"] = "Booking cancelled successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
