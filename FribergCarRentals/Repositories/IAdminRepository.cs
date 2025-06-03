using FribergCarRentals.Models;

namespace FribergCarRentals.Repositories
{
    public interface IAdminRepository
    {
        Admin? GetByEmailAndPassword(string email, string password);
    }
}

