using Microsoft.AspNetCore.Mvc;
using FribergCarRentals.Repositories;
using FribergCarRentals.Models;

namespace FribergCarRentals.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAdminRepository _adminRepo;
        private readonly ICustomerRepository _customerRepo;

        public AccountController(IAdminRepository adminRepo, ICustomerRepository customerRepo)
        {
            _adminRepo = adminRepo;
            _customerRepo = customerRepo;
        }

        [HttpGet]
        public IActionResult Login()
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
                return RedirectToAction("Index", "Admins");
            }

            var customer = _customerRepo.GetByEmailAndPassword(email, password);
            if (customer != null)
            {
                HttpContext.Session.SetString("CustomerName", customer.FirstName);
                HttpContext.Session.SetString(key: "CustomerId", customer.Id.ToString()); 
                return RedirectToAction("Index", "Cars", new { customerId = customer.Id });
            }

            ViewBag.Error = "Invalid email or password.";
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
            Login();

            return View();
        }

        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            _customerRepo.Add(customer);
            HttpContext.Session.SetString("CustomerName", customer.FirstName);
            HttpContext.Session.SetString("CustomerId", customer.Id.ToString()); 
            return RedirectToAction("Index", "Cars", new { customerId = customer.Id });
        }



    }
}
