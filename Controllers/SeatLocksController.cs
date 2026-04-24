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
    public class SeatLocksController : Controller
    {
        private readonly AppDbContext _context;

        public SeatLocksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SeatLocks
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.SeatLocks.Include(s => s.Screening).Include(s => s.Seat).Include(s => s.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: SeatLocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seatLock = await _context.SeatLocks
                .Include(s => s.Screening)
                .Include(s => s.Seat)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seatLock == null)
            {
                return NotFound();
            }

            return View(seatLock);
        }

        // GET: SeatLocks/Create
        public IActionResult Create()
        {
            ViewData["ScreeningId"] = new SelectList(_context.Screenings, "Id", "Id");
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Row");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: SeatLocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ScreeningId,SeatId,UserId,ExpiresAt,CreatedAt")] SeatLock seatLock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(seatLock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ScreeningId"] = new SelectList(_context.Screenings, "Id", "Id", seatLock.ScreeningId);
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Row", seatLock.SeatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", seatLock.UserId);
            return View(seatLock);
        }

        // GET: SeatLocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seatLock = await _context.SeatLocks.FindAsync(id);
            if (seatLock == null)
            {
                return NotFound();
            }
            ViewData["ScreeningId"] = new SelectList(_context.Screenings, "Id", "Id", seatLock.ScreeningId);
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Row", seatLock.SeatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", seatLock.UserId);
            return View(seatLock);
        }

        // POST: SeatLocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ScreeningId,SeatId,UserId,ExpiresAt,CreatedAt")] SeatLock seatLock)
        {
            if (id != seatLock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seatLock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeatLockExists(seatLock.Id))
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
            ViewData["ScreeningId"] = new SelectList(_context.Screenings, "Id", "Id", seatLock.ScreeningId);
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Row", seatLock.SeatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", seatLock.UserId);
            return View(seatLock);
        }

        // GET: SeatLocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seatLock = await _context.SeatLocks
                .Include(s => s.Screening)
                .Include(s => s.Seat)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seatLock == null)
            {
                return NotFound();
            }

            return View(seatLock);
        }

        // POST: SeatLocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seatLock = await _context.SeatLocks.FindAsync(id);
            if (seatLock != null)
            {
                _context.SeatLocks.Remove(seatLock);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeatLockExists(int id)
        {
            return _context.SeatLocks.Any(e => e.Id == id);
        }
    }
}
