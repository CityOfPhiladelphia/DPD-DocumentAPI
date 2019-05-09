using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DocumentAPI.Infrastructure.Interfaces;
using DocumentAPI.Infrastructure.Models;
using Newtonsoft.Json;

namespace DocumentAPI.Services
{
    public class QueryAppsServices : IQueryAppsServices
    {
        private readonly HttpClient _httpClient;

        public QueryAppsServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<QueryAppsResult> FilterQueryAppsResultByParameters(QueryAppsResult xTenderDocumentList, Category category)
        {
            var filteredAttributes = category.Attributes.Where(i => i.SelectedFilterType != null);
            foreach (var filteredAttribute in filteredAttributes)
            {
                if (filteredAttribute.Type.Name == DocumentCategories.TextTypeName)
                {
                    var stringFilter1 = filteredAttribute.FilterValue1;
                    switch (filteredAttribute.SelectedFilterType.Name)
                    {
                        case DocumentCategories.NumericEqualsOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => i.IndexValues[filteredAttribute.FieldNumber - 1].ToString() == stringFilter1).ToList();
                            break;
                        case DocumentCategories.NumericGreaterThanOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => i.IndexValues[filteredAttribute.FieldNumber - 1].ToString().StartsWith(stringFilter1)).ToList();
                            break;
                        case DocumentCategories.NumericLessThanOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => i.IndexValues[filteredAttribute.FieldNumber - 1].ToString().EndsWith(stringFilter1)).ToList();
                            break;
                        case DocumentCategories.NumericBetweenOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => i.IndexValues[filteredAttribute.FieldNumber - 1].ToString().Contains(stringFilter1)).ToList();
                            break;
                    }
                }
                else if (filteredAttribute.Type.Name == DocumentCategories.DateTypeName)
                {
                    var dateFilter1 = DateTime.TryParse(filteredAttribute.FilterValue1, out var date1Value) ? date1Value : DateTime.MinValue;
                    var dateFilter2 = DateTime.TryParse(filteredAttribute.FilterValue2, out var date2Value) ? date2Value : DateTime.MinValue;
                    DateTime ParsedIndexValue(Entry i) => DateTime.TryParse(i.IndexValues[filteredAttribute.FieldNumber - 1], out var dateIndexValue) ? dateIndexValue : DateTime.MinValue;

                    switch (filteredAttribute.SelectedFilterType.Name)
                    {
                        case DocumentCategories.DateEqualsOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => ParsedIndexValue(i) == dateFilter1).ToList();
                            break;
                        case DocumentCategories.DateGreaterThanOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => ParsedIndexValue(i) > dateFilter1).ToList();
                            break;
                        case DocumentCategories.DateLessThanOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => ParsedIndexValue(i) < dateFilter1).ToList();
                            break;
                        case DocumentCategories.DateBetweenOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => ParsedIndexValue(i) > dateFilter1 && ParsedIndexValue(i) < dateFilter2).ToList();
                            break;
                    }
                }
                else if (filteredAttribute.Type.Name == DocumentCategories.NumericTypeName)
                {
                    var numericFilter1 = int.TryParse(filteredAttribute.FilterValue1, out var int1Value) ? int1Value : -1;
                    var numericFilter2 = int.TryParse(filteredAttribute.FilterValue2, out var int2Value) ? int2Value : -1;
                    int ParsedIndexValue(Entry i) => int.TryParse(i.IndexValues[filteredAttribute.FieldNumber - 1].ToString(), out var intIndexValue) ? intIndexValue : -1;

                    switch (filteredAttribute.SelectedFilterType.Name)
                    {
                        case DocumentCategories.NumericEqualsOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => ParsedIndexValue(i) == numericFilter1).ToList();
                            break;
                        case DocumentCategories.NumericGreaterThanOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => ParsedIndexValue(i) > numericFilter1).ToList();
                            break;
                        case DocumentCategories.NumericLessThanOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => ParsedIndexValue(i) < numericFilter1).ToList();
                            break;
                        case DocumentCategories.NumericBetweenOperator:
                            xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => ParsedIndexValue(i) > numericFilter1 && ParsedIndexValue(i) < numericFilter2).ToList();
                            break;
                    }
                }
                else if (filteredAttribute.Type.Name == DocumentCategories.FullTextSearchName)
                {
                    var stringFilter1 = filteredAttribute.FilterValue1;

                    var fullTextFilteredDocuments = await FullTextRequestDocuments(category.Name, stringFilter1);

                    var filteredEntryIds = fullTextFilteredDocuments.Entries.Select(i => i.Id);

                    xTenderDocumentList.Entries = xTenderDocumentList.Entries.Where(i => filteredEntryIds.Contains(i.Id)).ToList();
                }
            }
            
            return xTenderDocumentList;
        }

        private async Task<QueryAppsResult> FullTextRequestDocuments(string categoryName, string fullTextSearch)
        {
            var category = DocumentCategories.Categories.SingleOrDefault(i => i.Name == categoryName);
            var adhocQueryObject = new RootObject
            {
                FullText = new FullText
                {
                    Value = fullTextSearch
                }
            };

            var adhocQueryJson = JsonConvert.SerializeObject(adhocQueryObject);

            var query = new UriBuilder($"{Config.AdHocQueryResultsPath}/{category.Id}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Config.Credentials);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.emc.ax+json"));

            var request = await _httpClient.PostAsync(query.Uri, new StringContent(adhocQueryJson, Encoding.UTF8, "application/vnd.emc.ax+json"));

            var result = JsonConvert.DeserializeObject<QueryAppsResult>(await request.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<QueryAppsResult> RequestDocuments(string categoryName)
        {
            var category = DocumentCategories.Categories.SingleOrDefault(i => i.Name == categoryName);
            var parameterString = string.Join(',', category?.Attributes.Select(i => "0")) ?? "0";
            var query = new UriBuilder($"{Config.RequestBasePath}/{Config.QueryAppsPath}/{category.Name}/{parameterString}/{Config.Credentials}");

            var request = await _httpClient.GetStringAsync(query.Uri);
            var result = JsonConvert.DeserializeObject<QueryAppsResult>(request);

            return result;
        }

        public async Task<Stream> GetResponse(HttpRequestMessage requestMessage)
        {
            var response = await _httpClient.SendAsync(requestMessage);

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
