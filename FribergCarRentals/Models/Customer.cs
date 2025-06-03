namespace FribergCarRentals.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // connection to bookings
        public List<Booking> Bookings { get; set; } = new();


    }
}
