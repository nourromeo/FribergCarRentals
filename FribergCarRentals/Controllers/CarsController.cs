using Microsoft.AspNetCore.Mvc;
using FribergCarRentals.Models;
using FribergCarRentals.Repositories;

namespace FribergCarRentals.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarRepository _carRepository;

        public CarsController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        // GET: Cars
        public IActionResult Index()
        {
            var cars = _carRepository.GetAll();

            ViewBag.CustomerName = TempData["CustomerName"];

            return View(cars);
        }

        // GET: Cars/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var car = _carRepository.GetById(id.Value);
            if (car == null) return NotFound();

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
        public IActionResult Create([Bind("Id,Brand,Model,ImageUrl")] Car car)
        {
            if (ModelState.IsValid)
            {
                _carRepository.Add(car);
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var car = _carRepository.GetById(id.Value);
            if (car == null) return NotFound();

            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Brand,Model,ImageUrl")] Car car)
        {
            if (id != car.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _carRepository.Update(car);
                }
                catch
                {
                    if (_carRepository.GetById(car.Id) == null)
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var car = _carRepository.GetById(id.Value);
            if (car == null) return NotFound();

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var car = _carRepository.GetById(id);
            if (car != null)
            {
                _carRepository.Delete(car);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
