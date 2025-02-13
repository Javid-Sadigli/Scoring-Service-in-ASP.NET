using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Scoring_Service.Data;
using Scoring_Service.Models.Dtos.Requests;
using Scoring_Service.Models.Dtos.Responses;
using Scoring_Service.Models.Entities;
using Scoring_Service.Services;
using Scoring_Service.Services.Interfaces;

namespace Scoring_Service.UnitTests.Services
{
    public class ScoringServiceTests
    {
        private readonly ScoringService scoringService; 
        private readonly Mock<DbSet<CustomerRequest>> customerRequestSetMock;
        private readonly Mock<DbSet<ConditionEvaulationResult>> conditionEvaluationResultSetMock;
        private readonly Mock<ApplicationDbContext> dbContextMock; 
        private readonly IEnumerable<Mock<ICondition>> conditionMocks;
        private readonly Mock<IMapper> mapperMock; 
        private readonly Mock<ILogger<ScoringService>> loggerMock;
    
        public ScoringServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            dbContextMock = new Mock<ApplicationDbContext>(options);
            customerRequestSetMock = new Mock<DbSet<CustomerRequest>>();
            conditionEvaluationResultSetMock = new Mock<DbSet<ConditionEvaulationResult>>();
            dbContextMock.Setup(db => db.CustomerRequests).Returns(customerRequestSetMock.Object);
            dbContextMock.Setup(db => db.EvaulationResults).Returns(conditionEvaluationResultSetMock.Object);
            dbContextMock.Setup(db => db.SaveChanges()).Returns(1);

            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<ScoringService>>();

            conditionMocks = new List<Mock<ICondition>>
            {
                new Mock<ICondition>(),
                new Mock<ICondition>(), 
                new Mock<ICondition>()
            };

            scoringService = new ScoringService(
                conditionMocks.Select(mock => mock.Object), 
                dbContextMock.Object, 
                mapperMock.Object, 
                loggerMock.Object
            );
        }

        [Fact]
        public void EvaluateCustomer_ShouldReturnEvaluation_WhenAllConditionsPassed()
        {
            CustomerRequestDto customerRequestDto = new CustomerRequestDto
            {
                FinCode = "777SA1",
                FirstName = "John",
                LastName = "Doe",
                Age = 30,
                Salary = 5000,
                Citizenship = "AZE"
            };

            CustomerRequest customerRequest = new CustomerRequest
            {
                FinCode = customerRequestDto.FinCode,
                FirstName = customerRequestDto.FirstName,
                LastName = customerRequestDto.LastName,
                Age = customerRequestDto.Age,
                Salary = customerRequestDto.Salary,
                Citizenship = customerRequestDto.Citizenship,
                Id = Guid.NewGuid(),
            };

            IEnumerable<ConditionEvaulationResult> conditionEvaluationResults = new List<ConditionEvaulationResult>
            {
                new ConditionEvaulationResult
                {
                    ConditionId = 1,
                    Amount = 1000, 
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = true, 
                    Id = Guid.NewGuid()
                },
                new ConditionEvaulationResult
                {
                    ConditionId = 3,
                    Amount = 2000,
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = true,
                    Id = Guid.NewGuid()
                },
                new ConditionEvaulationResult
                {
                    ConditionId = 2,
                    Amount = 500,
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = true,
                    Id = Guid.NewGuid()
                }
            };

            customerRequestSetMock.Setup(set => set.Add(customerRequest)).Verifiable();
            mapperMock.Setup(mapper => mapper.Map<CustomerRequest>(customerRequestDto)).Returns(customerRequest);

            for(int i = 0; i < conditionMocks.Count(); i++)
            {
                ConditionEvaulationResult evaulationResult = conditionEvaluationResults.ElementAt(i);

                conditionMocks.ElementAt(i)
                    .Setup(condition => condition.Evaluate(customerRequest))
                    .Returns(evaulationResult);

                conditionEvaluationResultSetMock.Setup(set => set.Add(evaulationResult)).Verifiable();
            }
            
            CustomerEvaluationResponseDto serviceResponse = scoringService.EvaluateCustomer(customerRequestDto); 

            customerRequestSetMock.Verify(set => set.Add(customerRequest), Times.Once);
            mapperMock.Verify(mapper => mapper.Map<CustomerRequest>(customerRequestDto), Times.Once);
            for(int i = 0; i < conditionMocks.Count(); i++)
            {
                conditionMocks.ElementAt(i).Verify(condition => condition.Evaluate(customerRequest), Times.Once);
                conditionEvaluationResultSetMock.Verify(set => set.Add(conditionEvaluationResults.ElementAt(i)), Times.Once);
            }
            
            Assert.NotNull(serviceResponse);
            Assert.Equal(customerRequest.FinCode, serviceResponse.CustomerFinCode);
            Assert.Equal(3500, serviceResponse.CreditAmount);
        }

        [Fact]
        public void EvaluateCustomer_ShouldReturnEvaluation_WhenOneOfConditionsIsNotPassed()
        {
            CustomerRequestDto customerRequestDto = new CustomerRequestDto
            {
                FinCode = "777SA1",
                FirstName = "John",
                LastName = "Doe",
                Age = 30,
                Salary = 500,
                Citizenship = "AZE"
            };

            CustomerRequest customerRequest = new CustomerRequest
            {
                FinCode = customerRequestDto.FinCode,
                FirstName = customerRequestDto.FirstName,
                LastName = customerRequestDto.LastName,
                Age = customerRequestDto.Age,
                Salary = customerRequestDto.Salary,
                Citizenship = customerRequestDto.Citizenship,
                Id = Guid.NewGuid(),
            };

            IEnumerable<ConditionEvaulationResult> conditionEvaluationResults = new List<ConditionEvaulationResult>
            {
                new ConditionEvaulationResult
                {
                    ConditionId = 1,
                    Amount = 1000, 
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = true, 
                    Id = Guid.NewGuid()
                },
                new ConditionEvaulationResult
                {
                    ConditionId = 3,
                    Amount = 0,
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = false,
                    Id = Guid.NewGuid()
                },
                new ConditionEvaulationResult
                {
                    ConditionId = 2,
                    Amount = 500,
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = true,
                    Id = Guid.NewGuid()
                }
            };

            customerRequestSetMock.Setup(set => set.Add(customerRequest)).Verifiable();
            mapperMock.Setup(mapper => mapper.Map<CustomerRequest>(customerRequestDto)).Returns(customerRequest);

            for(int i = 0; i < conditionMocks.Count(); i++)
            {
                ConditionEvaulationResult evaulationResult = conditionEvaluationResults.ElementAt(i);
                
                conditionMocks.ElementAt(i)
                    .Setup(condition => condition.Evaluate(customerRequest))
                    .Returns(evaulationResult);

                conditionEvaluationResultSetMock.Setup(set => set.Add(evaulationResult)).Verifiable();
            }
            
            CustomerEvaluationResponseDto serviceResponse = scoringService.EvaluateCustomer(customerRequestDto); 

            customerRequestSetMock.Verify(set => set.Add(customerRequest), Times.Once);
            mapperMock.Verify(mapper => mapper.Map<CustomerRequest>(customerRequestDto), Times.Once);
            for(int i = 0; i < conditionMocks.Count(); i++)
            {
                conditionMocks.ElementAt(i).Verify(condition => condition.Evaluate(customerRequest), Times.Once);
                conditionEvaluationResultSetMock.Verify(set => set.Add(conditionEvaluationResults.ElementAt(i)), Times.Once);
            }
            
            Assert.NotNull(serviceResponse);
            Assert.Equal(customerRequest.FinCode, serviceResponse.CustomerFinCode);
            Assert.Equal(1500, serviceResponse.CreditAmount);
        }

        [Fact]
        public void EvaluateCustomer_ShouldReturnEvaluation_WhenTwoOfConditionsAreNotPassed()
        {
            CustomerRequestDto customerRequestDto = new CustomerRequestDto
            {
                FinCode = "777SA1",
                FirstName = "John",
                LastName = "Doe",
                Age = 30,
                Salary = 500,
                Citizenship = "US"
            };

            CustomerRequest customerRequest = new CustomerRequest
            {
                FinCode = customerRequestDto.FinCode,
                FirstName = customerRequestDto.FirstName,
                LastName = customerRequestDto.LastName,
                Age = customerRequestDto.Age,
                Salary = customerRequestDto.Salary,
                Citizenship = customerRequestDto.Citizenship,
                Id = Guid.NewGuid(),
            };

            IEnumerable<ConditionEvaulationResult> conditionEvaluationResults = new List<ConditionEvaulationResult>
            {
                new ConditionEvaulationResult
                {
                    ConditionId = 1,
                    Amount = 1000, 
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = true, 
                    Id = Guid.NewGuid()
                },
                new ConditionEvaulationResult
                {
                    ConditionId = 3,
                    Amount = 0,
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = false,
                    Id = Guid.NewGuid()
                },
                new ConditionEvaulationResult
                {
                    ConditionId = 2,
                    Amount = 0,
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = false,
                    Id = Guid.NewGuid()
                }
            };

            customerRequestSetMock.Setup(set => set.Add(customerRequest)).Verifiable();
            mapperMock.Setup(mapper => mapper.Map<CustomerRequest>(customerRequestDto)).Returns(customerRequest);

            for(int i = 0; i < conditionMocks.Count(); i++)
            {
                ConditionEvaulationResult evaulationResult = conditionEvaluationResults.ElementAt(i);
                
                conditionMocks.ElementAt(i)
                    .Setup(condition => condition.Evaluate(customerRequest))
                    .Returns(evaulationResult);

                conditionEvaluationResultSetMock.Setup(set => set.Add(evaulationResult)).Verifiable();
            }
            
            CustomerEvaluationResponseDto serviceResponse = scoringService.EvaluateCustomer(customerRequestDto); 

            customerRequestSetMock.Verify(set => set.Add(customerRequest), Times.Once);
            mapperMock.Verify(mapper => mapper.Map<CustomerRequest>(customerRequestDto), Times.Once);
            for(int i = 0; i < conditionMocks.Count(); i++)
            {
                conditionMocks.ElementAt(i).Verify(condition => condition.Evaluate(customerRequest), Times.Once);
                conditionEvaluationResultSetMock.Verify(set => set.Add(conditionEvaluationResults.ElementAt(i)), Times.Once);
            }
            
            Assert.NotNull(serviceResponse);
            Assert.Equal(customerRequest.FinCode, serviceResponse.CustomerFinCode);
            Assert.Equal(1000, serviceResponse.CreditAmount);
        }

        [Fact]
        public void EvaluateCustomer_ShouldReturnEvaluation_WhenAllOfConditionsAreNotPassed()
        {
            CustomerRequestDto customerRequestDto = new CustomerRequestDto
            {
                FinCode = "777SA1",
                FirstName = "John",
                LastName = "Doe",
                Age = 17,
                Salary = 0,
                Citizenship = "US"
            };

            CustomerRequest customerRequest = new CustomerRequest
            {
                FinCode = customerRequestDto.FinCode,
                FirstName = customerRequestDto.FirstName,
                LastName = customerRequestDto.LastName,
                Age = customerRequestDto.Age,
                Salary = customerRequestDto.Salary,
                Citizenship = customerRequestDto.Citizenship,
                Id = Guid.NewGuid(),
            };

            IEnumerable<ConditionEvaulationResult> conditionEvaluationResults = new List<ConditionEvaulationResult>
            {
                new ConditionEvaulationResult
                {
                    ConditionId = 1,
                    Amount = 0, 
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = false, 
                    Id = Guid.NewGuid()
                },
                new ConditionEvaulationResult
                {
                    ConditionId = 3,
                    Amount = 0,
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = false,
                    Id = Guid.NewGuid()
                },
                new ConditionEvaulationResult
                {
                    ConditionId = 2,
                    Amount = 0,
                    CustomerRequestId = customerRequest.Id,
                    IsSatisfied = false,
                    Id = Guid.NewGuid()
                }
            };

            customerRequestSetMock.Setup(set => set.Add(customerRequest)).Verifiable();
            mapperMock.Setup(mapper => mapper.Map<CustomerRequest>(customerRequestDto)).Returns(customerRequest);

            for(int i = 0; i < conditionMocks.Count(); i++)
            {
                ConditionEvaulationResult evaulationResult = conditionEvaluationResults.ElementAt(i);
                
                conditionMocks.ElementAt(i)
                    .Setup(condition => condition.Evaluate(customerRequest))
                    .Returns(evaulationResult);

                conditionEvaluationResultSetMock.Setup(set => set.Add(evaulationResult)).Verifiable();
            }
            
            CustomerEvaluationResponseDto serviceResponse = scoringService.EvaluateCustomer(customerRequestDto); 

            customerRequestSetMock.Verify(set => set.Add(customerRequest), Times.Once);
            mapperMock.Verify(mapper => mapper.Map<CustomerRequest>(customerRequestDto), Times.Once);
            for(int i = 0; i < conditionMocks.Count(); i++)
            {
                conditionMocks.ElementAt(i).Verify(condition => condition.Evaluate(customerRequest), Times.Once);
                conditionEvaluationResultSetMock.Verify(set => set.Add(conditionEvaluationResults.ElementAt(i)), Times.Once);
            }
            
            Assert.NotNull(serviceResponse);
            Assert.Equal(customerRequest.FinCode, serviceResponse.CustomerFinCode);
            Assert.Equal(0, serviceResponse.CreditAmount);
        }
    }
}