namespace FribergCarRentels.Api.Dto
{
    public class BookingDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
        public int CarId { get; set; }
        public CarDto? Car { get; set; }
    }
}

