using FribergCarRentals.Models;
using FribergCarRentels.Api.Dto;

namespace FribergCarRentels.Api.Mappers
{
    public static class BookingMapper
    {
        public static BookingDto ToDto(this Booking b) => new BookingDto
        {
            Id = b.Id,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            CustomerId = b.CustomerId,
            CarId = b.CarId,
            Customer = b.Customer is null ? null : new CustomerDto
            {
                Id = b.Customer.Id,
                FirstName = b.Customer.FirstName,
                LastName = b.Customer.LastName
            },
            Car = b.Car is null ? null : new CarDto
            {
                Id = b.Car.Id,
                Brand = b.Car.Brand,
                Model = b.Car.Model,
                ImageUrl = b.Car.ImageUrl
            }
        };
    }
}
