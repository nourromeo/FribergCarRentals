using Microsoft.AspNetCore.Mvc;
using FribergCarRentals.Models;
using FribergCarRentals.Repositories;

namespace FribergCarRentals.Controllers
{
    public class AdminsController : Controller
    {
        private readonly IAdminRepository _adminRepository;

        public AdminsController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        // GET: Admins
        public IActionResult Index()
        {
            ViewBag.AdminName = TempData["AdminName"];

            return View();
        }

    }
}
