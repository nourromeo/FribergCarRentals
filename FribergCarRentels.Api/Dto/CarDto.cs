namespace FribergCarRentels.Api.Dto
{
    public class CarDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string CarFullName => $"{Brand} {Model}";
    }

}