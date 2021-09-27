using Microsoft.Extensions.Configuration;

namespace DocumentAPI.Infrastructure.Models
{
    public class Config
    {
        public string Credentials { get; set; }

        public string AdHocQueryResultsPath { get; set; }

        public string SelectIndexLookupPath { get; set; }

        public string OracleConnectionString { get; set; }

        public string ExportDocsJobPath { get; set; }

        public string ExportResultsPath { get; set; }

    }

    public static class ConfigExtensions
    {
        public static Config Load(this IConfiguration config)
        {
            return new Config
            {
                Credentials = config["Credentials"],
                AdHocQueryResultsPath = config["AdHocQueryResultsPath"],
                SelectIndexLookupPath = config["SelectIndexLookupPath"],
                OracleConnectionString = config["OracleConnectionString"],
                ExportDocsJobPath = config["ExportDocsJobPath"],
                ExportResultsPath = config["ExportResultsPath"],
            };
        }
    }
}
