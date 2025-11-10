using Microsoft.AspNetCore.Mvc;
using FribergCarRentals.Models;
using System.Text;
using System.Text.Json;

namespace FribergCarRentals.Controllers
{
    public class CustomersController : Controller
    {
        private readonly HttpClient _httpClient;

        public CustomersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("api");
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("customers");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to load customers list.";
                return View(new List<Customer>());
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var customers = JsonSerializer.Deserialize<List<Customer>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(customers);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"customers/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var responseBody = await response.Content.ReadAsStringAsync();
            var customer = JsonSerializer.Deserialize<Customer>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            var content = new StringContent(JsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("customers", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to create new customer.";
                return View(customer);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"customers/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var responseBody = await response.Content.ReadAsStringAsync();
            var customer = JsonSerializer.Deserialize<Customer>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(customer);

            var content = new StringContent(JsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"customers/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to update customer.";
                return View(customer);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"customers/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var responseBody = await response.Content.ReadAsStringAsync();
            var customer = JsonSerializer.Deserialize<Customer>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"customers/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to delete customer.";
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
