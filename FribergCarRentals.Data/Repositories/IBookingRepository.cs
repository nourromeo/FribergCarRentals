namespace FribergCarRentals.Repositories
{
    public interface IBookingRepository
    {
        List<Booking> GetAll();
        Booking? GetById(int id);
        void Add(Booking booking);
        void Update(Booking booking);
        void Delete(Booking booking);
        List<Booking> GetBookingsByCustomerId(int customerId);

    }
}
