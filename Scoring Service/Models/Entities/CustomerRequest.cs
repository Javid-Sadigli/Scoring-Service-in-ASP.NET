namespace Scoring_Service.Models.Entities
{
    public class CustomerRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? FinCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public string? Citizenship { get; set; }
    }
}
