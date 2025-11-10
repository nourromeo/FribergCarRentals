using FribergCarRentals.Models;

namespace FribergCarRentals.Repositories
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByEmailAsync(string email);

    }
}
