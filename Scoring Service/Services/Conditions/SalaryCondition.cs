using Microsoft.Extensions.Options;
using Scoring_Service.Configurations.Conditions;
using Scoring_Service.Models.Entities;
using Scoring_Service.Services.Interfaces;

namespace Scoring_Service.Services.Conditions
{
    public class SalaryCondition : ICondition
    {
        public string Id => "SALARY_CONDITION";

        private readonly SalaryConditionConfiguration configuration;

        public SalaryCondition(IOptions<SalaryConditionConfiguration> configuration)
        {
            this.configuration = configuration.Value; 
        }

        public ConditionEvaulationResult Evaluate(CustomerRequest customerRequest)
        {
            if(customerRequest.Salary >= configuration.Min)
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
