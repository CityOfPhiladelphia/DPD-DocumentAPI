using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace DocumentAPI.Infrastructure.Models
{
    public class QueryAppsResult
    {
        public string[] Columns { get; set; }

        public ICollection<Entry> Entries { get; set; } = new Collection<Entry>();
    }

    public class Entry
    {
        public int Id { get; set; }

        public int PageCount { get; set; }

        [JsonIgnore]
        public IList<string> IndexValues { get; set; }
    }

    public class ApiResult
    {
        public ICollection<FormattedEntry> Entries { get; } = new Collection<FormattedEntry>();
    }

    public class FormattedEntry : Entry
    {
        public new Dictionary<string, string> IndexValues { get; } = new Dictionary<string, string>();
    }

    public static class QueryAppsExtensions
    {
        public static ApiResult ToApiResult(this QueryAppsResult queryAppsResult)
        {
            var apiResult = new ApiResult();
            foreach (var result in queryAppsResult.Entries)
            {
                var formattedEntry = new FormattedEntry();
                for (var i = 0; i < result.IndexValues.Count; i++)
                {
                    var indexValue = result.IndexValues[i];
                    var indexProperty = queryAppsResult.Columns[i];

                    formattedEntry.Id = result.Id;
                    formattedEntry.PageCount = result.PageCount;
                    formattedEntry.IndexValues.Add(indexProperty, indexValue);
                }
                apiResult.Entries.Add(formattedEntry);
            }

            return apiResult;
        }

        public static Category BuildCategoryWithFilters(this Category documentCategory,
            string selectedFilterName1, string selectedFilterType1, string[] selectedFilterValues1,
            string selectedFilterName2 = null, string selectedFilterType2 = null, string[] selectedFilterValues2 = null)
        {

            foreach (var attribute in documentCategory.Attributes)
            {
                var betweenOperators = new Collection<string> { DepartmentEntities.DateBetweenOperator, DepartmentEntities.NumericBetweenOperator, DepartmentEntities.TextBetweenOperator };

                if (attribute.Name == selectedFilterName1)
                {
                    attribute.SelectedFilterType = new FilterType { Name = selectedFilterType1 };
                    attribute.FilterValue1 = selectedFilterValues1[0];
                    if (betweenOperators.Contains(selectedFilterType1))
                    {
                        attribute.FilterValue2 = selectedFilterValues1[1];
                    }
                }
                else if (attribute.Name == selectedFilterName2)
                {
                    attribute.SelectedFilterType = new FilterType { Name = selectedFilterType2 };
                    attribute.FilterValue1 = selectedFilterValues2[0];
                    if (betweenOperators.Contains(selectedFilterType2))
                    {
                        attribute.FilterValue2 = selectedFilterValues2[1];
                    }
                }
            }
            return documentCategory;
        }
    }
}
