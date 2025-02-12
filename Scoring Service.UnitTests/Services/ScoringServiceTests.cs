using AutoMapper;
using Moq;
using Scoring_Service.Data;
using Scoring_Service.Services;
using Scoring_Service.Services.Interfaces;

namespace Scoring_Service.UnitTests.Services
{
    public class ScoringServiceTests
    {
        private readonly ScoringService scoringService; 
        private readonly Mock<ApplicationDbContext> dbContextMock; 
        private readonly IEnumerable<Mock<ICondition>> conditionMocks;
        private readonly Mock<IMapper> mapperMock; 
        
    }
}