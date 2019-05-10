using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DocumentAPI.Infrastructure.Models
{
    public class QueryAppsResult
    {
        public string[] Columns { get; set; }

        public IEnumerable<Entry> Entries { get; set; }
    }

    public class Entry
    {
        public int Id { get; set; }

        public int PageCount { get; set; }

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
    }
}
