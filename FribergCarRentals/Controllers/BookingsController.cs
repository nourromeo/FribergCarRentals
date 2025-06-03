using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FribergCarRentals.Models;
using FribergCarRentals.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace FribergCarRentals.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;

        public BookingsController(
            IBookingRepository bookingRepository,
            ICarRepository carRepository,
            ICustomerRepository customerRepository)
        {
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
            _customerRepository = customerRepository;
        }

        public IActionResult Index()
        {
            var customerIdStr = HttpContext.Session.GetString("CustomerId");

            if (!string.IsNullOrEmpty(customerIdStr))
            {
                int customerId = int.Parse(customerIdStr);
                var bookings = _bookingRepository.GetBookingsByCustomerId(customerId);
                return View(bookings);
            }

            var allBookings = _bookingRepository.GetAll();
            return View(allBookings);
        }


        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = _bookingRepository.GetById(id.Value);
            if (booking == null) return NotFound();

            return View(booking);
        }

        public IActionResult Create()
        {
            var customerIdStr = HttpContext.Session.GetString("CustomerId");
            var adminName = HttpContext.Session.GetString("AdminName");

            ViewData["CarId"] = new SelectList(_carRepository.GetAll(), "Id", "Model");

            if (!string.IsNullOrEmpty(customerIdStr))
            {
                int customerId = int.Parse(customerIdStr);
                var booking = new Booking
                {
                    CustomerId = customerId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                };
                return View(booking);
            }

            if (!string.IsNullOrEmpty(adminName))
            {
                ViewData["CustomerId"] = new SelectList(_customerRepository.GetAll(), "Id", "Email");
                var booking = new Booking
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                };
                return View(booking);
            }

            return RedirectToAction("Login", "Account");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,StartDate,EndDate,CustomerId,CarId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _bookingRepository.Add(booking);
                return RedirectToAction(nameof(Index));
            }

            ViewData["CarId"] = new SelectList(_carRepository.GetAll(), "Id", "Model", booking.CarId);

            if (booking.CustomerId == 0)
                ViewData["CustomerId"] = new SelectList(_customerRepository.GetAll(), "Id", "Email");

            return View(booking);
        }



        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = _bookingRepository.GetById(id.Value);
            if (booking == null) return NotFound();

            ViewData["CarId"] = new SelectList(_carRepository.GetAll(), "Id", "Model", booking.CarId);
            ViewData["CustomerId"] = new SelectList(_customerRepository.GetAll(), "Id", "Email", booking.CustomerId);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,StartDate,EndDate,CustomerId,CarId")] Booking booking, string Customer_Email)
        {
            if (id != booking.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingBooking = _bookingRepository.GetById(booking.Id);
                if (existingBooking == null) return NotFound();

                existingBooking.StartDate = booking.StartDate;
                existingBooking.EndDate = booking.EndDate;
                existingBooking.CarId = booking.CarId;

                var customer = _customerRepository.GetById(existingBooking.CustomerId);
                if (customer != null)
                {
                    customer.Email = Customer_Email;
                    _customerRepository.Update(customer);
                }

                _bookingRepository.Update(existingBooking);

                return RedirectToAction(nameof(Index));
            }

            ViewData["CarId"] = new SelectList(_carRepository.GetAll(), "Id", "Model", booking.CarId);
            return View(booking);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = _bookingRepository.GetById(id.Value);
            if (booking == null) return NotFound();

            var customerIdStr = HttpContext.Session.GetString("CustomerId");
            var adminName = HttpContext.Session.GetString("AdminName");

            if (!string.IsNullOrEmpty(customerIdStr))
            {
                int customerId = int.Parse(customerIdStr);
                if (booking.CustomerId != customerId || booking.StartDate <= DateTime.Now)
                {
                    ViewBag.Error = "You can not delete old or current booking";
                }
            }

            return View(booking);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var booking = _bookingRepository.GetById(id);
            if (booking == null) return NotFound();

            var customerIdStr = HttpContext.Session.GetString("CustomerId");
            var adminName = HttpContext.Session.GetString("AdminName");

            if (!string.IsNullOrEmpty(customerIdStr))
            {
                int customerId = int.Parse(customerIdStr);
                if (booking.CustomerId != customerId || booking.StartDate <= DateTime.Now)
                {
                    ViewBag.Error = "You can not delete old or current booking";
                    return View("Delete", booking);
                }
            }

            _bookingRepository.Delete(booking);
            return RedirectToAction(nameof(Index));
        }


    }
}
