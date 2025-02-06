using Microsoft.AspNetCore.Mvc;
using Scoring_Service.Models.Dtos.Requests;
using Scoring_Service.Models.Dtos.Responses;
using Scoring_Service.Services;

namespace Scoring_Service.Controllers
{
    [Route("api/customer-evaluation")]
    [ApiController]
    public class ScoringController : ControllerBase
    {
        private readonly ScoringService scoringService;
        private readonly ILogger<ScoringController> logger;

        private static readonly string LOG_TEMPLATE = "{RequestMethod} request to /api/customer-evaluation{Endpoint}";

        public ScoringController(ScoringService scoringService, ILogger<ScoringController> logger)
        {
            this.scoringService = scoringService;
            this.logger = logger;
        }

        [HttpPost("evaluate")]
        public IActionResult EvaluateCustomer([FromBody] CustomerRequestDto customerRequest)
        {
            logger.LogInformation(LOG_TEMPLATE, "POST", "/evaluate");
            CustomerEvaluationResponseDto response = scoringService.EvaluateCustomer(customerRequest);
            return Ok(response);
        }
    }
}
