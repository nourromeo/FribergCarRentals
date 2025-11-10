using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FribergCarRentals.Controllers
{
    public class AdminsController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("api");
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            ViewBag.AdminName = HttpContext.Session.GetString("AdminName");

            var response = await _httpClient.GetAsync("admins");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Unable to load admin data.";
                return View();
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.Message = result?["message"];
            ViewBag.Role = result?["role"];

            return View();
        }
    }
}
