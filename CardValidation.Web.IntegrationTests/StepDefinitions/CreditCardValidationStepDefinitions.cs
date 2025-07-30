using CardValidation.ViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace CardValidation.Web.Tests.StepDefinitions
{
    [Binding]
    public class CreditCardValidationStepDefinitions : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ScenarioContext _scenarioContext;
        private Dictionary<string, object?> _cardDetails;

        public CreditCardValidationStepDefinitions(WebApplicationFactory<Program> factory, ScenarioContext scenarioContext)
        {
            _factory = factory;
            _scenarioContext = scenarioContext;
             _cardDetails = new Dictionary<string, object?>();
        }

         [Given(@"I have a valid credit card")]
        public void GivenIHaveAValidCreditCard()
        {
            _cardDetails = new Dictionary<string, object?>
            {
                { "Owner", "Valid Owner" },
                { "Number", "4111111111111111" },
                { "Date", DateTime.UtcNow.AddYears(2).ToString("MM/yy") },
                { "Cvv", "123" }
            };
        }

        [Given(@"I have a credit card with the following details:")]
        public void GivenIHaveAValidCreditCardWithTheFollowingDetails(Table table)
        {
            foreach (var row in table.Rows)
            {
                _cardDetails[row["Field"]] = row["Value"];
            }
        }

        [Given(@"the ""(.*)"" is ""(.*)""")]
        public void GivenTheFieldIs(string field, string value)
        {
            _cardDetails[field] = value;
        }

          [Given(@"the ""(.*)"" field is missing")]
        public void GivenTheFieldIsMissing(string field)
        {
            _cardDetails[field] = null;
        }

        [Given(@"I have a credit card with an invalid ""(.*)"" of ""(.*)"" and an invalid ""(.*)"" of ""(.*)""")]
        public void GivenIHaveACreditCardWithMultipleInvalidFields(string field1, string value1, string field2, string value2)
        {
            GivenIHaveAValidCreditCard();
            _cardDetails[field1] = value1;
            _cardDetails[field2] = value2;
        }

        [When(@"I request to validate the credit card")]
        public async Task WhenIRequestToValidateTheCreditCard()
        {
            var client = _factory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(_cardDetails), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/CardValidation/card/credit/validate", content);
            _scenarioContext["Response"] = response;
        }

        [Then(@"the response status code should be (.*)")]
        public void ThenTheResponseStatusCodeShouldBe(int statusCode)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            Assert.Equal((HttpStatusCode)statusCode, response.StatusCode);
        }

        [Then(@"the response body should be ""(.*)""")]
        public async Task ThenTheResponseBodyShouldBe(string expectedBody)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var body = await response.Content.ReadAsStringAsync();
            var paymentSystemType = JsonSerializer.Deserialize<Core.Enums.PaymentSystemType>(body);
            Assert.Equal(expectedBody, paymentSystemType.ToString());
        }

        [Then(@"the response should contain the error ""(.*)"" for the ""(.*)"" field")]
        public async Task ThenTheResponseShouldContainTheErrorForTheField(string expectedError, string field)
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var responseString = await response.Content.ReadAsStringAsync();
            var errors = JsonSerializer.Deserialize<Dictionary<string, string[]>>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(errors);
            Assert.True(errors.ContainsKey(field));
            Assert.Contains(expectedError, errors[field]);
        }
    }
}
