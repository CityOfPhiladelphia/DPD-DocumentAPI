using System.Collections.Generic;

namespace DocumentAPI.Infrastructure.Models
{
    public class FullText
    {
        public FullText(string value)
        {
            Value = value;
        }
        public int QueryOperator { get; set; } = 0;
        public int SearchType { get; set; } = 0;
        public bool Thesaurus { get; set; } = false;
        public string Value { get; set; }
    }

    public class Index
    {
        public Index(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsODMAField { get; set; } = false;
        public bool Searchable { get; set; } = true;
        public bool Displayable { get; set; } = true;
    }

    public class AdhocQueryRequest
    {
        public ICollection<Index> Indexes { get; set; } = new List<Index>();
        public FullText FullText { get; set; } = null;
        public object RetentionOptions { get; set; } = null;
        public object Name { get; set; } = null;
        public int Id { get; set; } = 0;
        public bool IsPublic { get; set; } = false;
        public bool IsIncludingPreviousRevisions { get; set; } = false;
        public object Owner { get; set; } = null;
        public int QueryType { get; set; } = 0;
        public object Apps { get; set; } = null;
    }
}