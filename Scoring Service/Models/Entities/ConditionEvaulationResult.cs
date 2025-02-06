namespace Scoring_Service.Models.Entities
{
    public class ConditionEvaulationResult
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ConditionId { get; set; }
        public bool IsSatisfied {  get; set; }
        public int Amount { get; set; }
        public Guid CustomerRequestId { get; set; }
    }
}
