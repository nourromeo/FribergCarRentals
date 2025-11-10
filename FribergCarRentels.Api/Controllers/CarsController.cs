using FribergCarRentals.Models;
using FribergCarRentals.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FribergCarRentals.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CarsController : ControllerBase
    {
        private readonly ICarRepository _carRepository;

        public CarsController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var cars = await _carRepository.GetAllAsync();
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null) 
                return NotFound();
            return Ok(car);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Car car)
        {
            if (string.IsNullOrWhiteSpace(car.Brand) || string.IsNullOrWhiteSpace(car.Model))
                return BadRequest(new { message = "Brand and Model are required." });

            await _carRepository.AddAsync(car);
            return Ok(car);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Car car)
        {
            if (id != car.Id)
                return BadRequest();

            await _carRepository.UpdateAsync(car);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
                return NotFound();

            await _carRepository.DeleteAsync(car);
            return NoContent();
        }
    }

}
