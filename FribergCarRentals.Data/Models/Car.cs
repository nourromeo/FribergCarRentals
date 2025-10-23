namespace FribergCarRentals.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }

        public string ImageUrl { get; set; }

        // One car > many bookings (One-to-Many)
        public List<Booking> Bookings { get; set; } = new();

    }
}
