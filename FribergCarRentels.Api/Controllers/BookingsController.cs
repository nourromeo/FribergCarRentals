using FribergCarRentals.Models;
using FribergCarRentals.Repositories;
using FribergCarRentels.Api.Dto;
using FribergCarRentels.Api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FribergCarRentals.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Customer")]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly ICarRepository _carRepo;
        private readonly ICustomerRepository _customerRepo;

        public BookingsController(IBookingRepository bookingRepo, ICarRepository carRepo, ICustomerRepository customerRepo)
        {
            _bookingRepo = bookingRepo;
            _carRepo = carRepo;
            _customerRepo = customerRepo;
        }

        // GET: api/bookings
        // Admin gets all bookings, Customer gets only his
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var customerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(role) && role == "Customer" && int.TryParse(customerIdClaim, out var cid))
            {
                var list = await _bookingRepo.GetBookingsByCustomerIdAsync(cid);
                return Ok(list.ConvertAll(b => b.ToDto()));
            }

            var all = await _bookingRepo.GetAllAsync();
            return Ok(all.ConvertAll(b => b.ToDto()));
        }


        // GET: api/bookings/{id}
        // Admin gets any booking, Customer gets only his
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null)
                return NotFound("Booking not found.");

            var role = User.FindFirstValue(ClaimTypes.Role);
            var customerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (role == "Customer" && customerIdClaim != null && booking.CustomerId.ToString() != customerIdClaim)
                return StatusCode(StatusCodes.Status403Forbidden, "You can only access your own bookings.");

            return Ok(booking.ToDto());
        }

        // POST: api/bookings
        // Only Admin or Customer for himself.
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Booking booking)
        {
            if (!ModelState.IsValid || booking.StartDate >= booking.EndDate)
                return BadRequest(new { message = "Invalid booking data." });

            var car = await _carRepo.GetByIdAsync(booking.CarId);
            if (car == null)
                return NotFound(new { message = "Car not found." });

            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (role == "Customer" && int.TryParse(userId, out var uid))
                booking.CustomerId = uid;

            var customer = await _customerRepo.GetByIdAsync(booking.CustomerId);
            if (customer == null)
                return NotFound(new { message = "Customer not found." });

            await _bookingRepo.AddAsync(booking);
            return Ok(new { message = "Booking created successfully", booking });
        }

        // PUT: api/bookings/{id}
        // Update booking (Admin or booking owner only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Booking updatedBooking)
        {
            if (id != updatedBooking.Id)
                return BadRequest(new { message = "Booking ID mismatch." });

            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (role == "Customer" && !int.TryParse(userIdClaim, out var uidParsed))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only edit your own bookings." });

            if (role == "Customer" && booking.CustomerId.ToString() != userIdClaim)
                return StatusCode(statusCode: StatusCodes.Status403Forbidden, new { message = "You can only edit your own bookings." });

            if (booking.StartDate <= DateTime.Now)
                return BadRequest(new { message = "You cannot edit current or past bookings." });

            booking.StartDate = updatedBooking.StartDate;
            booking.EndDate = updatedBooking.EndDate;
            booking.CarId = updatedBooking.CarId;

            await _bookingRepo.UpdateAsync(booking);
            return Ok(new { message = "Booking updated successfully", booking });
        }

        // DELETE: api/bookings/{id}
        // Delete booking (Admin or booking owner for his)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null)
                return NotFound("Booking not found.");

            var role = User.FindFirstValue(ClaimTypes.Role); 
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (role == "Customer" && !int.TryParse(userIdClaim, out var uidParsed))
                return StatusCode(StatusCodes.Status403Forbidden, "You can only delete your own bookings.");

            if (role == "Customer" && booking.CustomerId.ToString() != userIdClaim)
                return StatusCode(StatusCodes.Status403Forbidden, "You can only delete your own bookings.");

            if (booking.StartDate <= DateTime.Now)
                return BadRequest("Cannot delete active or past bookings.");

            await _bookingRepo.DeleteAsync(booking);
            return Ok(new { message = "Booking deleted successfully" });
        }

        // TEST: api/bookings/test-current-user
        [HttpGet("test-current-user")]
        [AllowAnonymous]
        public IActionResult GetCurrentUser()
        {
            var fullName = User.FindFirstValue("FullName");
            var role = User.FindFirstValue(ClaimTypes.Role);
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(new { fullName, role, id });
        }

    }
}
