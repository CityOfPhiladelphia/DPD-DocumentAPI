using System.Collections.Generic;

namespace DocumentAPI.Infrastructure.Models
{
    public class QueryAppsResult
    {
        public List<string> Columns { get; }

        public IList<Entry> Entries { get; set; }
    }

    public class Entry
    {
        public int Id { get; set; }

        public int PageCount { get; set; }

        public List<string> IndexValues { get; }
    }

    public class ApiResult
    {
        public List<FormattedEntry> Entries { get; } = new List<FormattedEntry>();
    }

    public class FormattedEntry : Entry
    {
        public new Dictionary<string, string> IndexValues { get; } = new Dictionary<string, string>();
    }

    public static class Extensions
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
    }
}
