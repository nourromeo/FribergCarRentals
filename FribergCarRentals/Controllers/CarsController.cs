using FribergCarRentals.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FribergCarRentals.Controllers
{
    public class CarsController : Controller
    {
        private readonly HttpClient _httpClient;

        public CarsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("api");
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("cars");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var cars = await response.Content.ReadFromJsonAsync<List<Car>>() ?? new List<Car>();
            ViewBag.CustomerName = TempData["CustomerName"];

            return View(cars);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();


            var response = await _httpClient.GetAsync($"cars/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var car = await response.Content.ReadFromJsonAsync<Car>();
            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (!ModelState.IsValid)
                return View(car);

            var response = await _httpClient.PostAsJsonAsync("cars", car);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to create new car.";
                return View(car);
            }

            return RedirectToAction(nameof(Index));

        }



        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();


            var response = await _httpClient.GetAsync($"cars/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var car = await response.Content.ReadFromJsonAsync<Car>();
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,ImageUrl")] Car car)
        {
            if (id != car.Id)
                return BadRequest();

            var response = await _httpClient.PutAsJsonAsync($"cars/{id}", car);
            if (!response.IsSuccessStatusCode)
                return View("Error");

            return RedirectToAction(nameof(Index));
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var response = await _httpClient.GetAsync($"cars/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var car = await response.Content.ReadFromJsonAsync<Car>();
            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var response = await _httpClient.DeleteAsync($"cars/{id}");
            if (!response.IsSuccessStatusCode)
                return View("Error");

            return RedirectToAction(nameof(Index));
        }
    }
}
