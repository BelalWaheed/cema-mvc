using CemaApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CemaApp.Services
{

    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKeyPrefix = "SeatLock";

        public BookingService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        private string GetCacheKey(int screeningId, int seatId) => $"{CacheKeyPrefix}:{screeningId}:{seatId}";

        public async Task<bool> LockSeatAsync(int screeningId, int seatId, string userId)
        {
            // 1. Check if seat is permanently booked in DB
            var isBooked = await _context.BookingSeats
                .AnyAsync(bs => bs.Booking.ScreeningId == screeningId
                             && bs.SeatId == seatId
                             && bs.Booking.Status == BookingStatus.Confirmed);

            if (isBooked) return false;

            var cacheKey = GetCacheKey(screeningId, seatId);

            // 2. Check if seat is locked in MemoryCache
            if (_cache.TryGetValue(cacheKey, out string existingUserId))
            {
                if (existingUserId != userId)
                {
                    return false; // Locked by someone else
                }
                // Already locked by this user, extend the 7-minute window
            }

            // 3. Set the lock with 7-minute expiration
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(7));

            _cache.Set(cacheKey, userId, cacheOptions);

            return true;
        }

        public async Task<bool> ConfirmBookingAsync(int screeningId, List<int> seatIds, string userId)
        {
            // 1. Verify locks still exist for this user in MemoryCache
            foreach (var seatId in seatIds)
            {
                var cacheKey = GetCacheKey(screeningId, seatId);
                if (!_cache.TryGetValue(cacheKey, out string? lockedUser) || lockedUser != userId)
                {
                    return false; // Lock expired or doesn't belong to user
                }
            }

            // 2. Proceed with DB booking
            var screening = await _context.Screenings.FindAsync(screeningId);
            if (screening == null) return false;

            var booking = new Booking
            {
                UserId = userId,
                ScreeningId = screeningId,
                BookingDate = DateTime.UtcNow,
                TotalPrice = seatIds.Count * screening.Price,
                Status = BookingStatus.Confirmed
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            foreach (var seatId in seatIds)
            {
                _context.BookingSeats.Add(new BookingSeat
                {
                    BookingId = booking.Id,
                    SeatId = seatId
                });

                // 3. Remove lock from cache after successful booking
                _cache.Remove(GetCacheKey(screeningId, seatId));
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SeatDto>> GetSeatsWithStatusAsync(int screeningId, string userId)
        {
            var screening = await _context.Screenings
                .Include(s => s.Hall)
                .ThenInclude(h => h.Seats)
                .FirstOrDefaultAsync(s => s.Id == screeningId);

            if (screening == null) return new List<SeatDto>();

            // Get all booked seats for this screening in one DB hit
            var bookedSeatIds = await _context.BookingSeats
                .Where(bs => bs.Booking.ScreeningId == screeningId && bs.Booking.Status == BookingStatus.Confirmed)
                .Select(bs => bs.SeatId)
                .ToListAsync();

            var seats = new List<SeatDto>();

            foreach (var seat in screening.Hall.Seats)
            {
                var state = SeatState.Available;

                if (bookedSeatIds.Contains(seat.Id))
                {
                    state = SeatState.Booked;
                }
                else
                {
                    var cacheKey = GetCacheKey(screeningId, seat.Id);
                    if (_cache.TryGetValue(cacheKey, out string? lockedUserId))
                    {
                        state = (lockedUserId == userId) ? SeatState.Selected : SeatState.Locked;
                    }
                }

                seats.Add(new SeatDto
                {
                    Id = seat.Id,
                    Row = seat.Row,
                    Number = seat.Number,
                    State = state.ToString()
                });
            }

            return seats;
        }
    }

}
