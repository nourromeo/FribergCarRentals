using FribergCarRentals.Models;
using FribergCarRentals.Repositories;
using FribergCarRentals.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FribergCarRentals.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAdminRepository _adminRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IConfiguration _configuration;

        public AccountController(IAdminRepository adminRepo, ICustomerRepository customerRepo, IConfiguration configuration)
        {
            _adminRepo = adminRepo;
            _customerRepo = customerRepo;
            _configuration = configuration;
        }

        // POST: api/Account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if user is Admin
            var admin = await _adminRepo.GetByEmailAsync(model.Email);
            if (admin != null && BCrypt.Net.BCrypt.Verify(model.Password, admin.Password))
            {
                var token = GenerateJwtToken(admin.Email, "Admin", admin.Id);
                return Ok(new
                {
                    token,
                    role = "Admin",
                    email = admin.Email
                });
            }

            // Check if user is Customer
            var customer = await _customerRepo.GetByEmailAsync(model.Email);
            if (customer != null && BCrypt.Net.BCrypt.Verify(model.Password, customer.Password))
            {
                var token = GenerateJwtToken(customer.Email, "Customer", customer.Id, customer.CustomerFullName);
                return Ok(new
                {
                    token,
                    role = "Customer",
                    name = customer.CustomerFullName,
                    id = customer.Id
                });
            }


            // Invalid login credentials
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // POST: api/Account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);
            await _customerRepo.AddAsync(customer);

            var token = GenerateJwtToken(customer.Email, "Customer", customer.Id, customer.CustomerFullName);

            return Ok(new
            {
                message = "Registration successful",
                token,
                role = "Customer",
                name = customer.CustomerFullName,
                id = customer.Id
            });
        }




        // JWT token generator
        private string GenerateJwtToken(string email, string role, int id, string? fullName = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            if (!string.IsNullOrEmpty(fullName))
                claims.Add(new Claim("FullName", fullName));

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
