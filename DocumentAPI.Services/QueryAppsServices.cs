using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DocumentAPI.Infrastructure.Interfaces;
using DocumentAPI.Infrastructure.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace DocumentAPI.Services
{
    public class QueryAppsServices : IQueryAppsServices
    {
        private readonly HttpClient _httpClient;
        private readonly Config _config;

        public QueryAppsServices(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config.Load();
        }

        public async Task<QueryAppsResult> FilterQueryAppsResultByParameters(Category category)
        {
            var filteredAttributes = category.Attributes.Where(i => i.SelectedFilterType != null);
            var adhocQueryObject = new AdhocQueryRequest();

            foreach (var filteredAttribute in filteredAttributes)
            {
                var query = "";
                if (filteredAttribute.Type.Name == DocumentCategories.TextTypeName)
                {
                    var stringFilter1 = filteredAttribute.FilterValue1;
                    var stringFilter2 = filteredAttribute.FilterValue2;

                    switch (filteredAttribute.SelectedFilterType.Name)
                    {
                        case DocumentCategories.NumericEqualsOperator:
                            query = $"{stringFilter1}";
                            break;
                        case DocumentCategories.NumericGreaterThanOperator:
                            query = $"Expression: > {stringFilter1}";
                            break;
                        case DocumentCategories.NumericLessThanOperator:
                            query = $"Expression: < {stringFilter1}";
                            break;
                        case DocumentCategories.NumericBetweenOperator:
                            query = $"Expression: ['{stringFilter1}','{stringFilter2}']";
                            break;
                    }
                }
                else if (filteredAttribute.Type.Name == DocumentCategories.DateTypeName)
                {
                    var dateFilter1 = (DateTime.TryParse(filteredAttribute.FilterValue1, out var date1Value) ? date1Value : DateTime.MinValue).ToShortDateString();
                    var dateFilter2 = (DateTime.TryParse(filteredAttribute.FilterValue2, out var date2Value) ? date2Value : DateTime.MinValue).ToShortDateString();

                    switch (filteredAttribute.SelectedFilterType.Name)
                    {
                        case DocumentCategories.DateEqualsOperator:
                            query = $"{dateFilter1}";
                            break;
                        case DocumentCategories.DateGreaterThanOperator:
                            query = $"Expression: > {dateFilter1}";
                            break;
                        case DocumentCategories.DateLessThanOperator:
                            query = $"Expression: < {dateFilter1}";
                            break;
                        case DocumentCategories.DateBetweenOperator:
                            query = $"Expression: ['{dateFilter1}','{dateFilter2}']";
                            break;
                    }
                }
                else if (filteredAttribute.Type.Name == DocumentCategories.NumericTypeName)
                {
                    var numericFilter1 = int.TryParse(filteredAttribute.FilterValue1, out var int1Value) ? int1Value : -1;
                    var numericFilter2 = int.TryParse(filteredAttribute.FilterValue2, out var int2Value) ? int2Value : -1;

                    switch (filteredAttribute.SelectedFilterType.Name)
                    {
                        case DocumentCategories.NumericEqualsOperator:
                            query = $"{numericFilter1}";
                            break;
                        case DocumentCategories.NumericGreaterThanOperator:
                            query = $"Expression: > {numericFilter1}";
                            break;
                        case DocumentCategories.NumericLessThanOperator:
                            query = $"Expression: < {numericFilter1}";
                            break;
                        case DocumentCategories.NumericBetweenOperator:
                            query = $"Expression: ['{numericFilter1}','{numericFilter2}']";
                            break;
                    }
                }
                else if (filteredAttribute.Type.Name == DocumentCategories.FullTextSearchName)
                {
                    query = filteredAttribute.FilterValue1;
                }

                adhocQueryObject.Indexes.Add(new Index
                {
                    Name = filteredAttribute.Name,
                    Value = query
                });
            }


            var xTenderDocumentList = await RequestDocuments(category.Name, adhocQueryObject);

            return xTenderDocumentList;
        }

        private async Task<QueryAppsResult> RequestDocuments(string categoryName, AdhocQueryRequest adhocQueryRequest)
        {
            var category = DocumentCategories.Categories.SingleOrDefault(i => i.Name == categoryName);
            var categoryId = category?.Id ?? 0;

            var adhocQueryJson = JsonConvert.SerializeObject(adhocQueryRequest);

            var query = new UriBuilder($"{_config.AdHocQueryResultsPath}/{categoryId}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _config.Credentials);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.emc.ax+json"));

            using (var request = await _httpClient.PostAsync(query.Uri, new StringContent(adhocQueryJson, Encoding.UTF8, "application/vnd.emc.ax+json")))
            {
                var result = JsonConvert.DeserializeObject<QueryAppsResult>(await request.Content.ReadAsStringAsync());
                var filteredResult = ExcludeNonPublicDocuments(result, category);
                return filteredResult;
            }
        }

        private QueryAppsResult ExcludeNonPublicDocuments(QueryAppsResult result, Category category)
        {
            var columnIndex = Array.FindIndex(result.Columns, i => i == category.NotPublicFieldName);

            if (columnIndex > -1)
            {
                result.Entries = result.Entries.Where(i =>
                    // parse and filter out entries where Not Public equals true
                    !(bool.Parse(i.IndexValues[columnIndex]))
                );
            };
            return result;
        }

        public HttpRequestMessage BuildDocumentRequest(string categoryName, int documentId)
        {
            var requestMessage = new HttpRequestMessage();
            var getFile = new UriBuilder($"{_config.RequestBasePath}/{_config.ExportDocumentPath}/{categoryName}/{documentId}/PDF/{_config.Credentials}");
            requestMessage.RequestUri = getFile.Uri;
            requestMessage.Method = HttpMethod.Get;

            return requestMessage;
        }

        public async Task<bool> CheckIfDocumentIsPublic(string categoryName, int documentId)
        {
            var documentCategory = DocumentCategories.Categories.SingleOrDefault(i => i.Name == categoryName);
            var adhocQueryRequest = new AdhocQueryRequest();
            var isPublicDocument = false;

            if (documentCategory != null)
            {
                if (!string.IsNullOrEmpty(documentCategory.NotPublicFieldName))
                {
                    adhocQueryRequest.Indexes.Add(new Index { Name = documentCategory.NotPublicFieldName, Value = "FALSE" });

                    var adhocQueryJson = JsonConvert.SerializeObject(adhocQueryRequest.Indexes);

                    var query = new UriBuilder($"{_config.SelectIndexLookupPath}/{documentCategory.Id}");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _config.Credentials);
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.emc.ax+json"));

                    using (var request = await _httpClient.PostAsync(query.Uri, new StringContent(adhocQueryJson, Encoding.UTF8, "application/vnd.emc.ax+json")))
                    {
                        var result = JsonConvert.DeserializeObject<QueryAppsResult>(await request.Content.ReadAsStringAsync());
                        var requestedRecord = result.Entries.SingleOrDefault(i => i.Id == documentId);
                        if (requestedRecord != null)
                        {
                            isPublicDocument = true;
                        }
                    }
                }
                else
                {
                    // no field signifying non-public document? then all documents are public
                    isPublicDocument = true;
                }
            }
            return isPublicDocument;
        }

        public async Task<Stream> GetResponse(HttpRequestMessage requestMessage)
        {
            var response = await _httpClient.SendAsync(requestMessage);
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
