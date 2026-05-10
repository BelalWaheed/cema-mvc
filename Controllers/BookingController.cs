using CemaApp.Models;
using CemaApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Microsoft.EntityFrameworkCore;

namespace CemaApp.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly AppDbContext _context;

        public BookingController(IBookingService bookingService, AppDbContext context)
        {
            _bookingService = bookingService;
            _context = context;
        }

        // AJAX endpoint to get all seats for a screening
        [HttpGet]
        public async Task<IActionResult> GetSeats(int screeningId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var seats = await _bookingService.GetSeatsWithStatusAsync(screeningId, userId);
            return Json(seats);
        }

        // AJAX endpoint to lock/unlock a seat
        [HttpPost]
        public async Task<IActionResult> ToggleSeat(int screeningId, int seatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            
            // Logic: Try to lock the seat
            // In a real app, you might want to "Unlock" if it's already selected by the same user.
            // But for simplicity, we'll just try to lock it.
            var success = await _bookingService.LockSeatAsync(screeningId, seatId, userId);
            
            return Json(new { success });
        }

        // Page to show seat selection
        [HttpGet]
        public async Task<IActionResult> SelectSeats(int screeningId)
        {
            var screening = await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == screeningId);

            if (screening == null) return NotFound();

            return View(screening);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(int screeningId, List<int> selectedSeatIds)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var success = await _bookingService.ConfirmBookingAsync(screeningId, selectedSeatIds, userId);

            if (success)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", "Could not confirm booking. Your selection may have expired.");
            return RedirectToAction("SelectSeats", new { screeningId });
        }
    }
}
