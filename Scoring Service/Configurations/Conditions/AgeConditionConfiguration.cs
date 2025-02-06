namespace Scoring_Service.Configurations.Conditions
{
    public class AgeConditionConfiguration
    {
        public int ConditionId { get; set; } = 1;
        public int Min {  get; set; }
        public int Max { get; set; }
        public int CreditAmount { get; set; }
    }
}
