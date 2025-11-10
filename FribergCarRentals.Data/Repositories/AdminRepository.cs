using FribergCarRentals.Models;
using FribergCarRentals.Data;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // البحث عن الأدمن عبر البريد فقط
        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
        }

    }
}
