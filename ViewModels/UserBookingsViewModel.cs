using CemaApp.Models;

namespace CemaApp.ViewModels
{
    public class UserBookingsViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
