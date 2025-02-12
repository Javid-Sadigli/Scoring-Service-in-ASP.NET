using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scoring_Service.Models.Dtos.Requests;
using Scoring_Service.Models.Dtos.Responses;

namespace Scoring_Service.IntegrationTests.Controllers
{
    public class ScoringControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly CustomWebApplicationFactory factory; 
        private readonly HttpClient client;

        public ScoringControllerTests(CustomWebApplicationFactory factory)
        {
            this.factory = factory;
            this.client = this.factory.CreateClient();
        }

        public void Dispose()
        {
            client.Dispose();
        }

        [Fact]
        public async Task PostCustomerEvaluation_WhenAgeAndSalaryIsSatisfied()
        {
            CustomerRequestDto customerRequest = new CustomerRequestDto
            {
                FinCode = "63SJF2",
                FirstName = "Fateh",
                LastName = "Sultanov",
                Age = 21,
                Salary = 3000,
                Citizenship = "TR"
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/customer-evaluation/evaluate", customerRequest);

            String responseBodyJson = await response.Content.ReadAsStringAsync();
            CustomerEvaluationResponseDto? responseBody = JsonConvert.DeserializeObject<CustomerEvaluationResponseDto>(responseBodyJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseBody);
            Assert.Equal(customerRequest.FinCode, responseBody.CustomerFinCode);
            Assert.Equal(3000, responseBody.CreditAmount);

        }

        [Fact]
        public async Task PostCustomerEvaluation_WhenAllOfConditionsIsSatisfied()
        {
            CustomerRequestDto customerRequest = new CustomerRequestDto
            {
                FinCode = "63SJF2",
                FirstName = "Fateh",
                LastName = "Sultanov",
                Age = 21,
                Salary = 3000,
                Citizenship = "AZE"
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/customer-evaluation/evaluate", customerRequest);

            String responseBodyJson = await response.Content.ReadAsStringAsync();
            CustomerEvaluationResponseDto? responseBody = JsonConvert.DeserializeObject<CustomerEvaluationResponseDto>(responseBodyJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseBody);
            Assert.Equal(customerRequest.FinCode, responseBody.CustomerFinCode);
            Assert.Equal(3500, responseBody.CreditAmount);
        }

        [Fact]
        public async Task PostCustomerEvaluation_WhenOnlyAgeIsSatisfied()
        {
            CustomerRequestDto customerRequest = new CustomerRequestDto
            {
                FinCode = "63SJF2",
                FirstName = "Fateh",
                LastName = "Sultanov",
                Age = 21,
                Salary = 800,
                Citizenship = "TR"
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/customer-evaluation/evaluate", customerRequest);

            String responseBodyJson = await response.Content.ReadAsStringAsync();
            CustomerEvaluationResponseDto? responseBody = JsonConvert.DeserializeObject<CustomerEvaluationResponseDto>(responseBodyJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseBody);
            Assert.Equal(customerRequest.FinCode, responseBody.CustomerFinCode);
            Assert.Equal(1000, responseBody.CreditAmount);
        }

        [Fact]
        public async Task PostCustomerEvaluation_WhenOnlySalaryIsSatisfied()
        {
            CustomerRequestDto customerRequest = new CustomerRequestDto
            {
                FinCode = "63SJF2",
                FirstName = "Fateh",
                LastName = "Sultanov",
                Age = 17,
                Salary = 3000,
                Citizenship = "TR"
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/customer-evaluation/evaluate", customerRequest);

            String responseBodyJson = await response.Content.ReadAsStringAsync();
            CustomerEvaluationResponseDto? responseBody = JsonConvert.DeserializeObject<CustomerEvaluationResponseDto>(responseBodyJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseBody);
            Assert.Equal(customerRequest.FinCode, responseBody.CustomerFinCode);
            Assert.Equal(2000, responseBody.CreditAmount);
        }

        [Fact]
        public async Task PostCustomerEvaluation_WhenOnlyCitizenshipIsSatisfied()
        {
            CustomerRequestDto customerRequest = new CustomerRequestDto
            {
                FinCode = "63SJF2",
                FirstName = "Fateh",
                LastName = "Sultanov",
                Age = 17,
                Salary = 500,
                Citizenship = "AZE"
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/customer-evaluation/evaluate", customerRequest);

            String responseBodyJson = await response.Content.ReadAsStringAsync();
            CustomerEvaluationResponseDto? responseBody = JsonConvert.DeserializeObject<CustomerEvaluationResponseDto>(responseBodyJson);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseBody);
            Assert.Equal(customerRequest.FinCode, responseBody.CustomerFinCode);
            Assert.Equal(500, responseBody.CreditAmount);
        }

    }
}
