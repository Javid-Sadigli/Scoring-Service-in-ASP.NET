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
        private readonly ILogger<AgeCondition> logger; 

        public AgeCondition(IOptions<AgeConditionConfiguration> configuration, ILogger<AgeCondition> logger)
        {
            this.configuration = configuration.Value;
            this.logger = logger;
        }

        public ConditionEvaulationResult Evaluate(CustomerRequest customerRequest)
        {
            if(customerRequest.Age >= configuration.Min && customerRequest.Age <= configuration.Max)
            {
                logger.LogInformation("Customer {CustomerId} has passed the evaluation of {ConditionId}", customerRequest.Id, this.Id);
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
                logger.LogInformation("Customer {CustomerId} has not passed the evaluation of {ConditionId}", customerRequest.Id, this.Id);
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
