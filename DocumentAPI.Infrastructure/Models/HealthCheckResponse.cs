using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentAPI.Infrastructure.Models
{
    public class HealthCheckBase
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
    public class HealthCheckResponse : HealthCheckBase
    {
        public List<HealthCheckResult> Results { get; set; } = new List<HealthCheckResult>();
    }

    public class HealthCheckResult : HealthCheckBase { }
}
