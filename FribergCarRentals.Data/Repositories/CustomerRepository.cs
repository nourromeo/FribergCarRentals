using FribergCarRentals.Models;
using FribergCarRentals.Data;
using System.Linq;

namespace FribergCarRentals.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }

        public Customer? GetById(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
        }

        public void Delete(Customer customer)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }

        public Customer? GetByEmailAndPassword(string email, string password)
        {
            return _context.Customers.FirstOrDefault(c => c.Email == email && c.Password == password);
        }

    }
}
