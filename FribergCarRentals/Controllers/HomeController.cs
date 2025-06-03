using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FribergCarRentals.Models;
using FribergCarRentals.Repositories;

namespace FribergCarRentals.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdminRepository _adminRepo;
        private readonly ICustomerRepository _customerRepo;

        public HomeController(IAdminRepository adminRepo, ICustomerRepository customerRepo)
        {
            _adminRepo = adminRepo;
            _customerRepo = customerRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var admin = _adminRepo.GetByEmailAndPassword(email, password);
            if (admin != null)
            {
                HttpContext.Session.SetString("AdminName", admin.Email);
                return RedirectToAction("Welcome", "Admins");
            }

            var customer = _customerRepo.GetByEmailAndPassword(email, password);
            if (customer != null)
            {
                HttpContext.Session.SetString("CustomerName", customer.FirstName);
                HttpContext.Session.SetString("CustomerId", customer.Id.ToString());
                return RedirectToAction("Index", "Bookings");
            }

            ViewBag.Error = "Fel e-post eller lösenord.";
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
