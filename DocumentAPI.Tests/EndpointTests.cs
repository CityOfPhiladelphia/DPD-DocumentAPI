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
        [InlineData("GET", "Historical_Commission")]
        [InlineData("POST", "Historical_Commission")]
        public async Task GetDocumentCategoriesTestAsync(string method, string entityName)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), $"/api/v1/document-request/document-categories/{entityName}");

            // Act
            using (var response = await _httpClient.SendAsync(request))
            {

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
        }

        [Theory]
        [InlineData("GET", "Historical_Commission", "HISTORICAL_COMM-CARD_CATALOG", 744)]
        [InlineData("GET", "Historical_Commission", "HISTORICAL_COMM-MEETING_MINUTES", 1882)]
        [InlineData("GET", "Historical_Commission", "HISTORICAL_COMM-MEETING_MINUTES", 743, false)]
        [InlineData("GET", "Historical_Commission", "HISTORICAL_COMM-PERMITS", 1233)]
        [InlineData("GET", "Historical_Commission", "HISTORICAL_COMM-POLAROIDS", 1)]
        [InlineData("GET", "Historical_Commission", "HISTORICAL_COMM-REGISTRY", 700)]
        public async Task GetDocumentTestAsync(string method, string entityName, string categoryName, int documentId, bool isPublic = true)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), $"/api/v1/document-request/get-document/{entityName}/{categoryName}/{documentId}");

            // Act
            using (var response = await _httpClient.SendAsync(request))
            {
                // Assert
                if (isPublic)
                {
                    response.EnsureSuccessStatusCode();
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
                else
                {
                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                }

            }
        }
    }
}
