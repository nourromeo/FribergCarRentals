using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FribergCarRentals.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var allCustomers = await _context.Customers.ToListAsync();
            return Ok(allCustomers);
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });

            return Ok(customer);
        }

        // POST: api/customers
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!string.IsNullOrWhiteSpace(customer.Password))
                customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Customer created successfully", customer });
        }

        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAsync(int id, [FromBody] Customer updatedCustomer)
        {
            if (id != updatedCustomer.Id)
                return BadRequest("Customer ID mismatch");

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound("Customer not found");

            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;
            customer.Email = updatedCustomer.Email;

            if (!string.IsNullOrWhiteSpace(updatedCustomer.Password))
                customer.Password = BCrypt.Net.BCrypt.HashPassword(updatedCustomer.Password);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Customer updated successfully", customer });
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Customer deleted successfully" });
        }
    }
}
