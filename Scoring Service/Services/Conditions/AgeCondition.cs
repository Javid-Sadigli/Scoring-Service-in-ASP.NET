using Microsoft.Extensions.Options;
using Prometheus;
using Scoring_Service.Configurations.Conditions;
using Scoring_Service.Models.Entities;
using Scoring_Service.Services.Interfaces;

namespace Scoring_Service.Services.Conditions
{
    public class AgeCondition : ICondition
    {
        public int Id { get; }
        private readonly AgeConditionConfiguration configuration;
        private readonly ILogger<AgeCondition> logger; 
        private readonly Counter evaluationCounter = Metrics
            .CreateCounter(
                "scoring_service_age_condition_evaluations_total", 
                "The number of evaluations on customer for Age Condition.", 
                new CounterConfiguration{LabelNames = new[] {"is_satisfied"}}
            );

        public AgeCondition(IOptions<AgeConditionConfiguration> configuration, ILogger<AgeCondition> logger)
        {
            this.configuration = configuration.Value;
            this.logger = logger;
            this.Id = this.configuration.ConditionId; 
        }

        public ConditionEvaulationResult Evaluate(CustomerRequest customerRequest)
        {
            if(customerRequest.Age >= configuration.Min && customerRequest.Age <= configuration.Max)
            {
                logger.LogInformation("Customer {CustomerId} has passed the evaluation of {ConditionId}", customerRequest.Id, this.Id);
                evaluationCounter.WithLabels("true").Inc();
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
                evaluationCounter.WithLabels("false").Inc();
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
