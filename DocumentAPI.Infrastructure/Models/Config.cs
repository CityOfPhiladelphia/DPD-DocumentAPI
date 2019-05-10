using Microsoft.Extensions.Configuration;

namespace DocumentAPI.Infrastructure.Models
{
    public class Config
    {
        public string Credentials { get; set; }

        public string RequestBasePath { get; set; }

        public string AdHocQueryResultsPath { get; set; }

        public string QueryAppsPath { get; set; }

        public string RetrieveDocumentPath { get; set; }

        public string ExportDocumentPath { get; set; }
    }

    public static class ConfigExtensions
    {
        public static Config Load(this IConfiguration config)
        {
            return new Config
            {
                Credentials = config["Credentials"],
                RequestBasePath = config["RequestBasePath"],
                AdHocQueryResultsPath = config["AdHocQueryResultsPath"],
                QueryAppsPath = config["QueryAppsPath"],
                RetrieveDocumentPath = config["RetrieveDocumentPath"],
                ExportDocumentPath = config["ExportDocumentPath"]
            };
        }
    }
}
