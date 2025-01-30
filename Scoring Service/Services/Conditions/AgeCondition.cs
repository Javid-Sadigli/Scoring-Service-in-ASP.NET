using Microsoft.Extensions.Options;
using Scoring_Service.Configurations.Conditions;
using Scoring_Service.Models.Entities;
using Scoring_Service.Services.Interfaces;

namespace Scoring_Service.Services.Conditions
{
    public class AgeCondition : ICondition
    {
        public string Id => "AGE_CONDITION";
        private readonly AgeConditionConfiguration configuration; 

        public AgeCondition(IOptions<AgeConditionConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }

        public ConditionEvaulationResult Evaluate(CustomerRequest customerRequest)
        {
            if(customerRequest.Age >= configuration.Min && customerRequest.Age <= configuration.Max)
            {
                return new ConditionEvaulationResult
                {
                    ConditionId = this.Id,
                    IsSatisfied = true,
                    Amount = configuration.CreditAmount, 
                    CustomerRequestId = customerRequest.Id
                }; 
            }
            else
            {
                return new ConditionEvaulationResult
                {
                    ConditionId = this.Id,
                    IsSatisfied = false,
                    Amount = 0, 
                    CustomerRequestId = customerRequest.Id
                };
            }
        }
    }
}
