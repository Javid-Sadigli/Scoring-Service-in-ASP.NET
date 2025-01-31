using Microsoft.AspNetCore.Http;
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

        public ScoringController(ScoringService scoringService)
        {
            this.scoringService = scoringService;
        }

        [HttpPost("evaluate")]
        public IActionResult EvaluateCustomer([FromBody] CustomerRequestDto customerRequest)
        {
            CustomerEvaluationResponseDto response = scoringService.EvaluateCustomer(customerRequest);
            return Ok(response);
        }
    }
}
