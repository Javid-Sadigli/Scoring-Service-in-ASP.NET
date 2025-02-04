using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Scoring_Service.Configurations.Conditions;
using Scoring_Service.Models.Entities;
using Scoring_Service.Services.Conditions;

namespace Scoring_Service.UnitTests.Services.Conditions
{
    public class AgeConditionTests
    {
        private readonly Mock<ILogger<AgeCondition>> loggerMock; 
        private readonly AgeCondition ageCondition;
        private readonly AgeConditionConfiguration configuration;

        public AgeConditionTests()
        {
            configuration = new AgeConditionConfiguration
            {
                Min = 18,
                Max = 60,
                CreditAmount = 1000
            };

            IOptions<AgeConditionConfiguration> config = Options.Create(configuration); 
            loggerMock = new Mock<ILogger<AgeCondition>>();
            ageCondition = new AgeCondition(config, loggerMock.Object);
        }

        [Fact]
        public void Evaluate_ShouldReturnSatisfied_WhenAgeIsWithinRange()
        {
            CustomerRequest customerRequest = new CustomerRequest
            {
                FinCode = "63SJF2",
                FirstName = "Fateh", 
                LastName = "Sultanov", 
                Age = 21, 
                Salary = 3000, 
                Citizenship = "TR",
                Id = Guid.NewGuid(),
            };

            ConditionEvaulationResult result = ageCondition.Evaluate(customerRequest);

            Assert.NotNull(result);
            Assert.Equal(ageCondition.Id, result.ConditionId);
            Assert.True(result.IsSatisfied);
            Assert.Equal(configuration.CreditAmount, result.Amount);
            Assert.Equal(customerRequest.Id, result.CustomerRequestId); 
        }

        [Fact]
        public void Evaluate_ShouldReturnNotSatisfied_WhenAgeIsAboveMax()
        {
            CustomerRequest customerRequest = new CustomerRequest
            {
                FinCode = "63SJF2",
                FirstName = "Fateh",
                LastName = "Sultanov",
                Age = 66,
                Salary = 3000,
                Citizenship = "TR",
                Id = Guid.NewGuid(),
            };

            ConditionEvaulationResult result = ageCondition.Evaluate(customerRequest);

            Assert.NotNull(result);
            Assert.Equal(ageCondition.Id, result.ConditionId);
            Assert.False(result.IsSatisfied);
            Assert.Equal(0, result.Amount);
            Assert.Equal(customerRequest.Id, result.CustomerRequestId);
        }

        [Fact]
        public void Evaluate_ShouldReturnNotSatisfied_WhenAgeIsBelowMin()
        {
            CustomerRequest customerRequest = new CustomerRequest
            {
                FinCode = "63SJF2",
                FirstName = "Fateh",
                LastName = "Sultanov",
                Age = 17,
                Salary = 3000,
                Citizenship = "TR",
                Id = Guid.NewGuid(),
            };

            ConditionEvaulationResult result = ageCondition.Evaluate(customerRequest);

            Assert.NotNull(result);
            Assert.Equal(ageCondition.Id, result.ConditionId);
            Assert.False(result.IsSatisfied);
            Assert.Equal(0, result.Amount);
            Assert.Equal(customerRequest.Id, result.CustomerRequestId);
        }
    }
}
