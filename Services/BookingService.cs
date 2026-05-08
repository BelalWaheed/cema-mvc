//using CemaApp.Models;

//namespace CemaApp.Services
//{
//    public class BookingService
//    {
//        public interface IBookingService
//        {
//            Task<bool> LockSeatAsync(int screeningId, int seatId, string userId);
//            Task<bool> ConfirmBookingAsync(int screeningId, List<int> seatIds, string userId);
//            Task<List<SeatDto>> GetSeatsWithStatusAsync(int screeningId, string userId);
//            Task CleanExpiredLocksAsync();
//        }

//        public class BookingService : IBookingService
//        {
//            private readonly ApplicationDbContext _context;

//            public BookingService(ApplicationDbContext context)
//            {
//                _context = context;
//            }

//            public async Task<bool> LockSeatAsync(int screeningId, int seatId, string userId)
//            {
//                // 1. Check if seat already booked (permanent)
//                var isBooked = await _context.BookingSeats
//                    .AnyAsync(bs => bs.Booking.ScreeningId == screeningId
//                                 && bs.SeatId == seatId
//                                 && bs.Booking.Status == "Confirmed");

//                if (isBooked)
//                    return false; // Seat permanently booked

//                // 2. Check if seat locked by someone else (temporary)
//                var existingLock = await _context.SeatLocks
//                    .FirstOrDefaultAsync(sl => sl.ScreeningId == screeningId
//                                            && sl.SeatId == seatId
//                                            && sl.ExpiresAt > DateTime.UtcNow);

//                if (existingLock != null && existingLock.UserId != userId)
//                    return false; // Locked by another user

//                // 3. If user already has a lock, extend it
//                if (existingLock != null && existingLock.UserId == userId)
//                {
//                    existingLock.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
//                    await _context.SaveChangesAsync();
//                    return true;
//                }

//                // 4. Create new lock for 10 minutes
//                var seatLock = new SeatLock
//                {
//                    ScreeningId = screeningId,
//                    SeatId = seatId,
//                    UserId = userId,
//                    ExpiresAt = DateTime.UtcNow.AddMinutes(10)
//                };

//                _context.SeatLocks.Add(seatLock);
//                await _context.SaveChangesAsync();

//                return true; // Lock successful
//            }

//            public async Task<bool> ConfirmBookingAsync(int screeningId, List<int> seatIds, string userId)
//            {
//                // 1. Verify all seats still locked by this user
//                foreach (var seatId in seatIds)
//                {
//                    var validLock = await _context.SeatLocks
//                        .AnyAsync(sl => sl.ScreeningId == screeningId
//                                     && sl.SeatId == seatId
//                                     && sl.UserId == userId
//                                     && sl.ExpiresAt > DateTime.UtcNow);

//                    if (!validLock)
//                        return false; // Lock expired or stolen
//                }

//                // 2. Get screening to calculate price
//                var screening = await _context.Screenings.FindAsync(screeningId);
//                if (screening == null)
//                    return false;

//                // 3. Create booking
//                var booking = new Booking
//                {
//                    UserId = userId,
//                    ScreeningId = screeningId,
//                    BookingDate = DateTime.UtcNow,
//                    TotalPrice = seatIds.Count * screening.Price,
//                    Status = "Confirmed"
//                };

//                _context.Bookings.Add(booking);
//                await _context.SaveChangesAsync();

//                // 4. Create BookingSeat records
//                foreach (var seatId in seatIds)
//                {
//                    _context.BookingSeats.Add(new BookingSeat
//                    {
//                        BookingId = booking.Id,
//                        SeatId = seatId
//                    });
//                }

//                // 5. Delete the locks (no longer needed)
//                var locks = await _context.SeatLocks
//                    .Where(sl => sl.ScreeningId == screeningId
//                              && seatIds.Contains(sl.SeatId)
//                              && sl.UserId == userId)
//                    .ToListAsync();

//                _context.SeatLocks.RemoveRange(locks);
//                await _context.SaveChangesAsync();

//                return true;
//            }

//            public async Task<List<SeatDto>> GetSeatsWithStatusAsync(int screeningId, string userId)
//            {
//                var screening = await _context.Screenings
//                    .Include(s => s.Hall)
//                    .ThenInclude(h => h.Seats)
//                    .FirstOrDefaultAsync(s => s.Id == screeningId);

//                if (screening == null)
//                    return new List<SeatDto>();

//                var seats = new List<SeatDto>();

//                foreach (var seat in screening.Hall.Seats)
//                {
//                    var state = await GetSeatStateAsync(screeningId, seat.Id, userId);

//                    seats.Add(new SeatDto
//                    {
//                        Id = seat.Id,
//                        Row = seat.Row,
//                        Number = seat.Number,
//                        State = state.ToString()
//                    });
//                }

//                return seats;
//            }

//            private async Task<SeatState> GetSeatStateAsync(int screeningId, int seatId, string currentUserId)
//            {
//                // Check if permanently booked
//                var isBooked = await _context.BookingSeats
//                    .AnyAsync(bs => bs.Booking.ScreeningId == screeningId
//                                 && bs.SeatId == seatId
//                                 && bs.Booking.Status == "Confirmed");

//                if (isBooked)
//                    return SeatState.Booked;

//                // Check if locked
//                var seatLock = await _context.SeatLocks
//                    .FirstOrDefaultAsync(sl => sl.ScreeningId == screeningId
//                                            && sl.SeatId == seatId
//                                            && sl.ExpiresAt > DateTime.UtcNow);

//                if (seatLock != null)
//                {
//                    if (seatLock.UserId == currentUserId)
//                        return SeatState.Selected; // User's own lock
//                    else
//                        return SeatState.Locked; // Someone else's lock
//                }

//                return SeatState.Available;
//            }

//            public async Task CleanExpiredLocksAsync()
//            {
//                var expiredLocks = await _context.SeatLocks
//                    .Where(sl => sl.ExpiresAt <= DateTime.UtcNow)
//                    .ToListAsync();

//                _context.SeatLocks.RemoveRange(expiredLocks);
//                await _context.SaveChangesAsync();
//            }
//        }

//        // DTO for returning seat info to view
//        public class SeatDto
//        {
//            public int Id { get; set; }
//            public string Row { get; set; }
//            public int Number { get; set; }
//            public string State { get; set; } // "Available", "Selected", "Locked", "Booked"
//        }

//        public enum SeatState
//        {
//            Available,
//            Selected,
//            Locked,
//            Booked
//        }
//    }
//}
