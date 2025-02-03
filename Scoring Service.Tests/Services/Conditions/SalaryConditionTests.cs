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
    public class SalaryConditionTests
    {
        private readonly Mock<ILogger<SalaryCondition>> loggerMock; 
        private readonly SalaryCondition salaryCondition;
        private readonly SalaryConditionConfiguration configuration;

        public SalaryConditionTests()
        {
            configuration = new SalaryConditionConfiguration
            {
                CreditAmount = 2000,
                Min = 1000
            };

            IOptions<SalaryConditionConfiguration> config = Options.Create(configuration);
            loggerMock = new Mock<ILogger<SalaryCondition>>();
            salaryCondition = new SalaryCondition(config, loggerMock.Object);
        }

        [Fact]
        public void Evaluate_ShouldReturnSatisfied_WhenSalaryIsEnough()
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

            ConditionEvaulationResult result = salaryCondition.Evaluate(customerRequest);

            Assert.NotNull(result);
            Assert.Equal(salaryCondition.Id, result.ConditionId);
            Assert.True(result.IsSatisfied);
            Assert.Equal(configuration.CreditAmount, result.Amount);
            Assert.Equal(customerRequest.Id, result.CustomerRequestId);
        }

        [Fact]
        public void Evaluate_ShouldReturnNotSatisfied_WhenSalaryIsNotEnough()
        {
            CustomerRequest customerRequest = new CustomerRequest
            {
                FinCode = "63SJF2",
                FirstName = "Fateh",
                LastName = "Sultanov",
                Age = 21,
                Salary = 400,
                Citizenship = "TR",
                Id = Guid.NewGuid(),
            };

            ConditionEvaulationResult result = salaryCondition.Evaluate(customerRequest);

            Assert.NotNull(result);
            Assert.Equal(salaryCondition.Id, result.ConditionId);
            Assert.False(result.IsSatisfied);
            Assert.Equal(0, result.Amount);
            Assert.Equal(customerRequest.Id, result.CustomerRequestId);
        }
    }
}
