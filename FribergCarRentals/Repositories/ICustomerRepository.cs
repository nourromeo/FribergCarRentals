using FribergCarRentals.Models;

namespace FribergCarRentals.Repositories
{
    public interface ICustomerRepository
    {
        List<Customer> GetAll();
        Customer? GetById(int id);
        Customer? GetByEmailAndPassword(string email, string password); 
        void Add(Customer customer);
        void Update(Customer customer);
        void Delete(Customer customer);
    }
}
