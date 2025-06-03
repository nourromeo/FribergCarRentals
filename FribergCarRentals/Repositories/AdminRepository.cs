using FribergCarRentals.Models;
using FribergCarRentals.Data;
using System.Linq;

namespace FribergCarRentals.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Admin? GetByEmailAndPassword(string email, string password)
        {
            return _context.Admins.FirstOrDefault(a => a.Email == email && a.Password == password);
        }
    }
}

