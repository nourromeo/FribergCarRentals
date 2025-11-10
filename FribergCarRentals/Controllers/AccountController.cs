using Microsoft.AspNetCore.Mvc;
using FribergCarRentals.Data.Dtos;
using FribergCarRentals.Models;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace FribergCarRentals.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("api");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("account/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
            {
                ViewBag.Error = "Unexpected server response.";
                return View();
            }

            // Save JWT and user info in Session
            HttpContext.Session.SetString("JwtToken", result.Token);
            HttpContext.Session.SetString("Role", result.Role);

            if (result.Role == "Admin")
            {
                HttpContext.Session.SetString("AdminName", result.Email);
                return RedirectToAction("Index", "Admins");
            }
            else if (result.Role == "Customer")
            {
                HttpContext.Session.SetString("CustomerName", result.Name);
                HttpContext.Session.SetString("CustomerId", result.Id.ToString());
                return RedirectToAction("Index", "Cars");
            }

            ViewBag.Error = "Unexpected role.";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            var content = new StringContent(JsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("account/register", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Registration failed. Try again.";
                return View(customer);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
            {
                ViewBag.Error = "Unexpected server response.";
                return View(customer);
            }

            HttpContext.Session.SetString("JwtToken", result.Token);
            HttpContext.Session.SetString("Role", result.Role);
            HttpContext.Session.SetString("CustomerName", result.Name);
            HttpContext.Session.SetString("CustomerId", result.Id.ToString());

            Console.WriteLine($"TOKEN STORED: {result.Token}");

            return RedirectToAction("Index", "Cars");
        }

    }
}
