using AutoMapper;
using Scoring_Service.Data;
using Scoring_Service.Models.Dtos.Requests;
using Scoring_Service.Models.Dtos.Responses;
using Scoring_Service.Models.Entities;
using Scoring_Service.Services.Interfaces;

namespace Scoring_Service.Services
{
    public class ScoringService
    {
        private readonly IEnumerable<ICondition> conditions;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper; 

        public ScoringService(
            IEnumerable<ICondition> conditions,
            ApplicationDbContext dbContext, 
            IMapper mapper
        ){
            this.conditions = conditions;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public CustomerEvaluationResponseDto EvaluateCustomer(CustomerRequestDto customerRequestDto)
        {
            CustomerRequest customerRequest = mapper.Map<CustomerRequest>(customerRequestDto);

            dbContext.CustomerRequests.Add(customerRequest);
            dbContext.SaveChanges();

            CustomerEvaluationResponseDto response = new CustomerEvaluationResponseDto();
            response.CustomerFinCode = customerRequest.FinCode;
            response.EvaulationResults = new List<ConditionEvaulationResult>();
            response.CreditAmount = 0; 

            foreach (ICondition condition in conditions)
            {
                ConditionEvaulationResult evaulationResult = condition.Evaluate(customerRequest);

                response.EvaulationResults.Add(evaulationResult);

                if (evaulationResult.IsSatisfied)
                {
                    response.CreditAmount += evaulationResult.Amount;
                }

                dbContext.EvaulationResults.Add(evaulationResult);
            }

            dbContext.SaveChanges();
            return response;
        }
    }
}
