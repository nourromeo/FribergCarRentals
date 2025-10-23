using FribergCarRentals.Models;
using FribergCarRentals.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Booking> GetAll()
        {
            return _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .ToList();
        }

        public Booking? GetById(int id)
        {
            return _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .FirstOrDefault(b => b.Id == id);
        }

        public void Add(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        public void Update(Booking booking)
        {
            _context.Bookings.Update(booking);
            _context.SaveChanges();
        }

        public void Delete(Booking booking)
        {
            _context.Bookings.Remove(booking);
            _context.SaveChanges();
        }

        public List<Booking> GetBookingsByCustomerId(int customerId)
        {
            return _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .Where(b => b.CustomerId == customerId)
                .ToList();
        }

    }
}
