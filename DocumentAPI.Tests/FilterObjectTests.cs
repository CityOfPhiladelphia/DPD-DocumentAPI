using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using DocumentAPI.Infrastructure.Models;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System;

namespace DocumentAPI.Tests
{
    public class FilterObjectTests
    {
        private readonly HttpClient _httpClient;

        public FilterObjectTests()
        {
            var server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(config => config.AddUserSecrets<Startup>())
                .UseEnvironment("Development")
                .UseStartup<Startup>());

            _httpClient = server.CreateClient();
        }

        [Theory]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "DOCUMENT DATE", "BEFORE", new[] { "1/1/00" })]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "MEETING NUMBER", "EQUALS", new[] { "318" })]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "MEETING NUMBER", "GREATER THAN", new[] { "100" })]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "MEETING NUMBER", "BETWEEN", new[] { "100", "150" })]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "FULL_TEXT", "FULL_TEXT", new[] { "Spring Garden" })]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "FULL_TEXT", "FULL_TEXT", new[] { "City Hall" })]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "FULL_TEXT", "FULL_TEXT", new[] { "The Committee deferred a decision" })]
        public async Task GetFilteredDocumentList_BySingleParameter_ShouldReturnResults(string categoryName,
            string selectedFilterName1, string selectedFilterType1, string[] selectedFilterValues)
        {
            var documentCategory = DocumentCategories.Categories.SingleOrDefault(i => i.Name == categoryName);
            documentCategory.BuildCategoryWithFilters(selectedFilterName1, selectedFilterType1, selectedFilterValues);

            var apiResultTest = await MakeFilterDocumentRequest(documentCategory);

            // Should have results
            Assert.True(apiResultTest.Entries.Any());

            // None of the results should be marked "not public"
            Assert.False(apiResultTest.Entries.Where(i => bool.Parse(i.IndexValues[documentCategory.NotPublicFieldName])).Any());
        }

        [Theory]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "DOCUMENT TYPE", "EQUALS", new[] { "Archive" })]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "FULL_TEXT", "FULL_TEXT", new[] { "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce eget diam nulla. Proin nec velit ut massa faucibus commodo." })]
        public async Task GetFilteredDocumentList_NonPublic_ShouldNotReturnResults(string categoryName,
            string selectedFilterName, string selectedFilterType, string[] selectedFilterValues)
        {
            var documentCategory = DocumentCategories.Categories.SingleOrDefault(i => i.Name == categoryName);
            documentCategory.BuildCategoryWithFilters(selectedFilterName, selectedFilterType, selectedFilterValues);

            var apiResultTest = await MakeFilterDocumentRequest(documentCategory);
            Assert.False(apiResultTest.Entries.Any());
        }

        [Theory]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "MEETING NUMBER", "GREATER THAN", new[] { "100" }, "DOCUMENT DATE", "LESS THAN", new[] { "1/1/1975" })]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "MEETING NUMBER", "BETWEEN", new[] { "102", "155" }, "DOCUMENT DATE", "BETWEEN", new[] { "1/1/1970", "5/1/1970" })]
        [InlineData("HISTORICAL_COMM-MEETING_MINUTES", "FULL_TEXT", "FULL_TEXT", new[] { "Vine Street" }, "DOCUMENT DATE", "BETWEEN", new[] { "1/1/1988", "5/1/1988" })]
        public async Task GetFilteredDocumentList_ByMultipleParameters_ShouldReturnResults(string categoryName,
                            string selectedFilterName1, string selectedFilterType1, string[] selectedFilterValues1,
                            string selectedFilterName2, string selectedFilterType2, string[] selectedFilterValues2)
        {
            var documentCategory = DocumentCategories.Categories.SingleOrDefault(i => i.Name == categoryName);
            documentCategory.BuildCategoryWithFilters(selectedFilterName1, selectedFilterType1, selectedFilterValues1, selectedFilterName2, selectedFilterType2, selectedFilterValues2);

            var apiResultTest = await MakeFilterDocumentRequest(documentCategory);

            // Should have results
            Assert.True(apiResultTest.Entries.Any());

            // None of the results should be marked "not public"
            Assert.False(apiResultTest.Entries.Where(i => bool.Parse(i.IndexValues[documentCategory.NotPublicFieldName])).Any());
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
