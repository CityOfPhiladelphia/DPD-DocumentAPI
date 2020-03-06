using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using DocumentAPI.Infrastructure.Models;
using DocumentAPI.Infrastructure.Interfaces;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using DocumentAPI.Services;

namespace DocumentAPI.Tests
{
    public class FilterObjectTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _httpClient;
        private readonly IQueryAppsServices _queryAppsServices;
        public FilterObjectTests()
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(config => config.AddUserSecrets<Startup>())
                .ConfigureServices(services =>
                {
                    services.AddScoped<IQueryAppsServices, QueryAppsServices>();
                })
                .UseEnvironment("Development")
                .UseStartup<Startup>();
            _server = new TestServer(webHostBuilder);

            _httpClient = _server.CreateClient();
            _queryAppsServices = _server.Host.Services.GetRequiredService<IQueryAppsServices>();
        }

        [Theory]
        [InlineData(1, 7, "DOCUMENT DATE", "BEFORE", new[] { "1/1/00" })]
        [InlineData(1, 7, "MEETING NUMBER", "EQUALS", new[] { "318" })]
        [InlineData(1, 7, "MEETING NUMBER", "GREATER THAN", new[] { "100" })]
        [InlineData(1, 7, "MEETING NUMBER", "BETWEEN", new[] { "100", "150" })]
        [InlineData(1, 7, "FULL_TEXT", "FULL_TEXT", new[] { "Spring Garden" })]
        [InlineData(1, 7, "FULL_TEXT", "FULL_TEXT", new[] { "City Hall" })]
        [InlineData(1, 7, "FULL_TEXT", "FULL_TEXT", new[] { "The Committee deferred a decision" })]
        public async Task GetFilteredDocumentList_BySingleParameter_ShouldReturnResults(int entityId, int categoryId,
            string selectedFilterName1, string selectedFilterType1, string[] selectedFilterValues)
        {
            var entities = await _queryAppsServices.GetEntities();
            var entity = entities.SingleOrDefault(i => i.Id == entityId);
            var category = entity.Categories.SingleOrDefault(i => i.Id == categoryId);
            category.BuildCategoryWithFilters(selectedFilterName1, selectedFilterType1, selectedFilterValues);

            var apiResultTest = await MakeFilterDocumentRequest(category);

            // Should have results
            Assert.True(apiResultTest.Entries.Any());

            // None of the results should be marked "not public"
            Assert.False(apiResultTest.Entries.Where(i => bool.Parse(i.IndexValues[category.NotPublicFieldName])).Any());
        }

        [Theory]
        [InlineData(1, 7, "DOCUMENT TYPE", "EQUALS", new[] { "Archive" })]
        [InlineData(1, 7, "FULL_TEXT", "FULL_TEXT", new[] { "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce eget diam nulla. Proin nec velit ut massa faucibus commodo." })]
        public async Task GetFilteredDocumentList_NonPublic_ShouldNotReturnResults(int entityId, int categoryId,
            string selectedFilterName, string selectedFilterType, string[] selectedFilterValues)
        {
            var entities = await _queryAppsServices.GetEntities();
            var entity = entities.SingleOrDefault(i => i.Id == entityId);
            var category = entity.Categories.SingleOrDefault(i => i.Id == categoryId);
            category.BuildCategoryWithFilters(selectedFilterName, selectedFilterType, selectedFilterValues);

            var apiResultTest = await MakeFilterDocumentRequest(category);
            Assert.False(apiResultTest.Entries.Any());
        }

        [Theory]
        [InlineData(1, 7, "MEETING NUMBER", "GREATER THAN", new[] { "100" }, "DOCUMENT DATE", "LESS THAN", new[] { "1/1/1975" })]
        [InlineData(1, 7, "MEETING NUMBER", "BETWEEN", new[] { "102", "155" }, "DOCUMENT DATE", "BETWEEN", new[] { "1/1/1970", "5/1/1970" })]
        [InlineData(1, 7, "FULL_TEXT", "FULL_TEXT", new[] { "Vine Street" }, "DOCUMENT DATE", "BETWEEN", new[] { "1/1/1988", "5/1/1988" })]
        public async Task GetFilteredDocumentList_ByMultipleParameters_ShouldReturnResults(int entityId, int categoryId,
                            string selectedFilterName1, string selectedFilterType1, string[] selectedFilterValues1,
                            string selectedFilterName2, string selectedFilterType2, string[] selectedFilterValues2)
        {
            var entities = await _queryAppsServices.GetEntities();
            var entity = entities.SingleOrDefault(i => i.Id == entityId);
            var category = entity.Categories.SingleOrDefault(i => i.Id == categoryId);
            category.BuildCategoryWithFilters(selectedFilterName1, selectedFilterType1, selectedFilterValues1, selectedFilterName2, selectedFilterType2, selectedFilterValues2);

            var apiResultTest = await MakeFilterDocumentRequest(category);

            // Should have results
            Assert.True(apiResultTest.Entries.Any());

            // None of the results should be marked "not public"
            Assert.False(apiResultTest.Entries.Where(i => bool.Parse(i.IndexValues[category.NotPublicFieldName])).Any());
        }

        private async Task<ApiResult> MakeFilterDocumentRequest(Category documentCategory)
        {
            var request = new HttpRequestMessage(new HttpMethod(HttpMethod.Post.Method), $"/api/v1/document-request/filtered-document-list");
            var json = JsonConvert.SerializeObject(documentCategory);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            request.Content = stringContent;
            using (var response = await _httpClient.SendAsync(request))
            {
                return JsonConvert.DeserializeObject<ApiResult>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}