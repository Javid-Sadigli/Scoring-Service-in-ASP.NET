using Scoring_Service.Models.Entities;

namespace Scoring_Service.Models.Dtos.Responses
{
    public class CustomerEvaluationResponseDto
    {
        public string? CustomerFinCode { get; set; }
        public int CreditAmount { get; set; }
        public List<ConditionEvaulationResult>? EvaulationResults { get; set; }
    }
}
