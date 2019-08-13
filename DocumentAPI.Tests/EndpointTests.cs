using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DocumentAPI.Tests
{
    public class EndpointTests
    {
        private readonly HttpClient _httpClient;

        public EndpointTests()
        {
            var server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(config => config.AddUserSecrets<Startup>())
                .UseEnvironment("Development")
                .UseStartup<Startup>());

            _httpClient = server.CreateClient();

        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        public async Task GetDocumentCategoriesTestAsync(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/v1/document-request/document-categories");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            if (response.IsSuccessStatusCode)
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
            else
            {
                Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
            }
        }


        [Theory]
        [InlineData("GET", "HISTORICAL_COMM-CARD_CATALOG")]
        [InlineData("GET", "HISTORICAL_COMM-MEETING_MINUTES")]
        [InlineData("GET", "HISTORICAL_COMM-PERMITS")]
        [InlineData("GET", "HISTORICAL_COMM-POLAROIDS")]
        [InlineData("GET", "HISTORICAL_COMM-REGISTRY")]

        public async Task GetDocumentListTestAsync(string method, string categoryName)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), $"/api/v1/document-request/document-list/{categoryName}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("GET", "HISTORICAL_COMM-CARD_CATALOG", 1)]
        [InlineData("GET", "HISTORICAL_COMM-MEETING_MINUTES", 1)]
        [InlineData("GET", "HISTORICAL_COMM-PERMITS", 1)]
        [InlineData("GET", "HISTORICAL_COMM-POLAROIDS", 1)]
        [InlineData("GET", "HISTORICAL_COMM-REGISTRY", 1)]
        public async Task GetDocumentTestAsync(string method, string categoryName, int documentId)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), $"/api/v1/document-request/get-document/{categoryName}/{documentId}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
