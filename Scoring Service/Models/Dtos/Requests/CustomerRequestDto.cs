namespace Scoring_Service.Models.Dtos.Requests
{
    public class CustomerRequestDto
    {
        public string? FinCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public string? Citizenship { get; set; }
    }
}
