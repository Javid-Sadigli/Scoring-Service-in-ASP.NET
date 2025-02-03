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

namespace Scoring_Service.Tests.Services.Conditions
{
    public class CitizenshipConditionTests
    {
        private readonly Mock<ILogger<CitizenshipCondition>> loggerMock;
        private readonly CitizenshipConditionConfiguration configuration;
        private readonly CitizenshipCondition citizenshipCondition;

        public CitizenshipConditionTests()
        {
            configuration = new CitizenshipConditionConfiguration
            {
                Value = "AZE",
                CreditAmount = 500
            };

            IOptions<CitizenshipConditionConfiguration> config = Options.Create(configuration);
            loggerMock = new Mock<ILogger<CitizenshipCondition>>();
            citizenshipCondition = new CitizenshipCondition(config, loggerMock.Object);
        }

        [Fact]
        public void Evaluate_ShouldReturnSatisfied_WhenCitizenshipIsValid()
        {
            CustomerRequest customerRequest = new CustomerRequest
            {
                FinCode = "63SJF2",
                FirstName = "Fateh",
                LastName = "Sultanov",
                Age = 21,
                Salary = 3000,
                Citizenship = "AZE",
                Id = Guid.NewGuid(),
            };

            ConditionEvaulationResult result = citizenshipCondition.Evaluate(customerRequest);

            Assert.NotNull(result);
            Assert.Equal(citizenshipCondition.Id, result.ConditionId);
            Assert.True(result.IsSatisfied);
            Assert.Equal(configuration.CreditAmount, result.Amount);
            Assert.Equal(customerRequest.Id, result.CustomerRequestId);
        }

        [Fact]
        public void Evaluate_ShouldReturnNotSatisfied_WhenCitizenshipIsInvalid()
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

            ConditionEvaulationResult result = citizenshipCondition.Evaluate(customerRequest);

            Assert.NotNull(result);
            Assert.Equal(citizenshipCondition.Id, result.ConditionId);
            Assert.False(result.IsSatisfied);
            Assert.Equal(0, result.Amount);
            Assert.Equal(customerRequest.Id, result.CustomerRequestId);
        }
    }
}
