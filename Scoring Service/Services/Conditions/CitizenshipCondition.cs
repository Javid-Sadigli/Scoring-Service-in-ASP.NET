using Microsoft.Extensions.Options;
using Scoring_Service.Configurations.Conditions;
using Scoring_Service.Models.Entities;
using Scoring_Service.Services.Interfaces;

namespace Scoring_Service.Services.Conditions
{
    public class CitizenshipCondition : ICondition
    {
        public int Id { get; }

        private readonly CitizenshipConditionConfiguration configuration;
        private readonly ILogger<CitizenshipCondition> logger;

        public CitizenshipCondition(IOptions<CitizenshipConditionConfiguration> configuration, ILogger<CitizenshipCondition> logger)
        {
            this.configuration = configuration.Value;
            this.logger = logger;
            this.Id = this.configuration.ConditionId;
        }

        public ConditionEvaulationResult Evaluate(CustomerRequest customerRequest)
        {
            if (customerRequest.Citizenship.ToUpper().Equals(configuration.Value.ToUpper()))
            {
                logger.LogInformation("Customer {CustomerId} has passed the evaluation of {ConditionId}", customerRequest.Id, this.Id);
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
                logger.LogInformation("Customer {CustomerId} has not passed the evaluation of {ConditionId}", customerRequest.Id, this.Id);
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
