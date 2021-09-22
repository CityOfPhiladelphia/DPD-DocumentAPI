using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentAPI.Infrastructure.Models
{
    public class JobTokenResult
    {
        public string JobToken { get; set; }

        public int Status { get; set; }

        public List<Link> Links { get; set; }
    }

    public class Link
    {
        public string Href { get; set; }
    }
}
