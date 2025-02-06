using Scoring_Service.Models.Entities;

namespace Scoring_Service.Services.Interfaces
{
    public interface ICondition
    {
        public int Id { get; }
        public ConditionEvaulationResult Evaluate(CustomerRequest customerRequest);
    }
}
