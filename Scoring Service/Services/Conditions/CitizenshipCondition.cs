using Microsoft.Extensions.Options;
using Scoring_Service.Configurations.Conditions;
using Scoring_Service.Models.Entities;
using Scoring_Service.Services.Interfaces;

namespace Scoring_Service.Services.Conditions
{
    public class CitizenshipCondition : ICondition
    {
        public string Id => "CITIZENSHIP_CONDITION";

        private readonly CitizenshipConditionConfiguration configuration;

        public CitizenshipCondition(IOptions<CitizenshipConditionConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }

        public ConditionEvaulationResult Evaluate(CustomerRequest customerRequest)
        {
            if (customerRequest.Citizenship.ToUpper().Equals(configuration.Value.ToUpper()))
            {
                return new ConditionEvaulationResult
                {
                    Amount = configuration.CreditAmount,
                    ConditionId = this.Id,
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = true
                }; 
            }
            else
            {
                return new ConditionEvaulationResult
                {
                    Amount = 0,
                    ConditionId = this.Id,
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = false
                };
            }
        }
    }
}
