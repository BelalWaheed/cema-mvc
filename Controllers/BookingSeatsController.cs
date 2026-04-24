using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cema.Data;
using cema.Models;

namespace cema.Controllers
{
    public class BookingSeatsController : Controller
    {
        private readonly AppDbContext _context;

        public BookingSeatsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: BookingSeats
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.BookingSeats.Include(b => b.Booking).Include(b => b.Seat);
            return View(await appDbContext.ToListAsync());
        }

        // GET: BookingSeats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingSeat = await _context.BookingSeats
                .Include(b => b.Booking)
                .Include(b => b.Seat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookingSeat == null)
            {
                return NotFound();
            }

            return View(bookingSeat);
        }

        // GET: BookingSeats/Create
        public IActionResult Create()
        {
            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "Status");
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Row");
            return View();
        }

        // POST: BookingSeats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookingId,SeatId,PriceAtBooking")] BookingSeat bookingSeat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookingSeat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "Status", bookingSeat.BookingId);
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Row", bookingSeat.SeatId);
            return View(bookingSeat);
        }

        // GET: BookingSeats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingSeat = await _context.BookingSeats.FindAsync(id);
            if (bookingSeat == null)
            {
                return NotFound();
            }
            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "Status", bookingSeat.BookingId);
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Row", bookingSeat.SeatId);
            return View(bookingSeat);
        }

        // POST: BookingSeats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookingId,SeatId,PriceAtBooking")] BookingSeat bookingSeat)
        {
            if (id != bookingSeat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookingSeat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingSeatExists(bookingSeat.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookingId"] = new SelectList(_context.Bookings, "Id", "Status", bookingSeat.BookingId);
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Row", bookingSeat.SeatId);
            return View(bookingSeat);
        }

        // GET: BookingSeats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingSeat = await _context.BookingSeats
                .Include(b => b.Booking)
                .Include(b => b.Seat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookingSeat == null)
            {
                return NotFound();
            }

            return View(bookingSeat);
        }

        // POST: BookingSeats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookingSeat = await _context.BookingSeats.FindAsync(id);
            if (bookingSeat != null)
            {
                _context.BookingSeats.Remove(bookingSeat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingSeatExists(int id)
        {
            return _context.BookingSeats.Any(e => e.Id == id);
        }
    }
}
