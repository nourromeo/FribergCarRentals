namespace FribergCarRentels.Api.Dto
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CustomerFullName => $"{FirstName} {LastName}";
    }
}