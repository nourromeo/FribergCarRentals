using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FribergCarRentals.Models;
using FribergCarRentals.Data.Dtos;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FribergCarRentals.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("api");
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: Home/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both email and password.";
                return View("Index");
            }

            var loginDto = new LoginDto { Email = email, Password = password };

            var content = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync("account/login", content);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Server connection failed: {ex.Message}";
                return View("Index");
            }

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password.";
                return View("Index");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                ViewBag.Error = "Unexpected server response.";
                return View("Index");
            }

            // Create Claims based on user data
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, result.Email ?? ""),
        new Claim(ClaimTypes.Role, result.Role ?? "")
    };

            // Build a ClaimsIdentity with the JWT scheme
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user within the MVC app (creates authentication cookie)
            await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, principal);

            // Store token temporarily in Session for API requests
            HttpContext.Session.SetString("JwtToken", result.Token);

            // Redirect based on user role
            if (result.Role == "Admin")
                return RedirectToAction("Index", "Admins");
            else if (result.Role == "Customer")
                return RedirectToAction("Index", "Bookings");

            ViewBag.Error = "Unknown user role.";
            return View("Index");
        }



        // Diagnostic route to confirm token presence
        [HttpGet]
        public IActionResult SessionToken()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var role = HttpContext.Session.GetString("Role");
            var msg = $"Token present: {!string.IsNullOrEmpty(token)} | Role: {role}";
            Debug.WriteLine(msg);
            return Content(msg);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
