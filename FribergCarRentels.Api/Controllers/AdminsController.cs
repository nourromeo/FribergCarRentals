using FribergCarRentals.Data;
using FribergCarRentals.Models;
using FribergCarRentals.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FribergCarRentals.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;

        public AdminsController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        // GET: api/Admins
        [HttpGet]
        public IActionResult Index()
        {
            var adminEmail = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

            var role = User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

            return Ok(new
            {
                message = $"Welcome, {adminEmail}!",
                role
            });
        }

        // creating a new admin, temporary open endpoint
        [HttpPost("create-admin-test")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAdminTest([FromServices] ApplicationDbContext db, [FromBody] Admin admin)
        {
            if (admin == null)
                return BadRequest("Admin data is required.");

            if (await db.Admins.AnyAsync(a => a.Email == admin.Email))
                return BadRequest(new { message = "Admin with this email already exists." });

            admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);

            db.Admins.Add(admin);
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Test admin created successfully",
                email = admin.Email
            });
        }
    }
}
