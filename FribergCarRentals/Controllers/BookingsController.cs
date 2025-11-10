using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FribergCarRentals.Models;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace FribergCarRentals.Controllers
{
    public class BookingsController : Controller
    {
        private readonly HttpClient _httpClient;

        public BookingsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("api");
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("bookings");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var bookings = await response.Content.ReadFromJsonAsync<List<Booking>>() ?? new List<Booking>();

            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"bookings/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var booking = await response.Content.ReadFromJsonAsync<Booking>();
            return View(booking);
        }


        // GET: Bookings/Create
        public async Task<IActionResult> Create()
        {
            var carsResponse = await _httpClient.GetAsync("cars");
            if (!carsResponse.IsSuccessStatusCode)
                throw new Exception($"Cars API returned {carsResponse.StatusCode}");

            var cars = await carsResponse.Content.ReadFromJsonAsync<List<Car>>() ?? new List<Car>();
            ViewData["CarId"] = new SelectList(cars, "Id", "CarFullName");

            var role = HttpContext.Session.GetString("Role");
            var customerId = HttpContext.Session.GetString("CustomerId");
            var customerName = HttpContext.Session.GetString("CustomerName");

            // Customer
            if (role == "Customer")
            {
                if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(customerName))
                    return RedirectToAction("Login", "Account");

                ViewData["CustomerId"] = new SelectList(
                    new List<SelectListItem>
                    {
                new SelectListItem { Value = customerId, Text = customerName }
                    },
                    "Value", "Text"
                );
            }

            // Admin
            else if (role == "Admin")
            {
                var customersResponse = await _httpClient.GetAsync("customers");
                if (!customersResponse.IsSuccessStatusCode)
                    throw new Exception($"Customers API returned {customersResponse.StatusCode}");

                var customers = await customersResponse.Content.ReadFromJsonAsync<List<Customer>>() ?? new List<Customer>();
                ViewData["CustomerId"] = new SelectList(customers, "Id", "CustomerFullName");
            }

            return View(new Booking());
        }


        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            var response = await _httpClient.PostAsJsonAsync("bookings", booking);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Index));
        }


        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"bookings/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var booking = await response.Content.ReadFromJsonAsync<Booking>();

            var carsResponse = await _httpClient.GetAsync("cars");
            if (carsResponse.IsSuccessStatusCode)
            {
                var cars = await carsResponse.Content.ReadFromJsonAsync<List<Car>>();
                ViewData["CarId"] = new SelectList(cars, "Id", "CarFullName", booking.CarId);
            }

            return View(booking);
        }


        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            var response = await _httpClient.PutAsJsonAsync($"bookings/{id}", booking);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Index));
        }


        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"bookings/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var booking = await response.Content.ReadFromJsonAsync<Booking>();
            return View(booking);
        }


        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"bookings/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return View("Error");
        }

    }
}
