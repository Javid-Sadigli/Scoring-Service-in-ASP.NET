namespace Scoring_Service.Configurations
{
    public class RateLimitingConfiguration
    {
        public string? PolicyName { get; set; } = "fixed";
        public int PermitLimit { get; set; }
        public int WindowSeconds { get; set; }
        public int QueueLimit { get; set; }
    }
}