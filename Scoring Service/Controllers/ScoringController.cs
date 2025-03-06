using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Prometheus;
using Scoring_Service.Models.Dtos.Requests;
using Scoring_Service.Models.Dtos.Responses;
using Scoring_Service.Services;

namespace Scoring_Service.Controllers
{
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Route("api/customer-evaluation")]
    public class ScoringController : ControllerBase
    {
        private readonly ScoringService scoringService;
        private readonly ILogger<ScoringController> logger;

        private static readonly string LOG_TEMPLATE = "{@RequestMethod} request to /api/customer-evaluation{@Endpoint}";
        private static readonly Histogram scoringDurationHistogram = Metrics
            .CreateHistogram(
                "scoring_service_scoring_duration",
                "The duration of all scoring",
                new HistogramConfiguration
                {
                    Buckets = Histogram.LinearBuckets(0, 100, 10)
                }
            );

        public ScoringController(ScoringService scoringService, ILogger<ScoringController> logger)
        {
            this.scoringService = scoringService;
            this.logger = logger;
        }

        [HttpPost("evaluate")]
        public IActionResult EvaluateCustomer([FromBody] CustomerRequestDto customerRequest)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            logger.LogInformation(LOG_TEMPLATE, "POST", "/evaluate");
            CustomerEvaluationResponseDto response = scoringService.EvaluateCustomer(customerRequest);
            stopwatch.Stop();
            scoringDurationHistogram.Observe(stopwatch.ElapsedMilliseconds);
            return Ok(response);
        }
    }
}
