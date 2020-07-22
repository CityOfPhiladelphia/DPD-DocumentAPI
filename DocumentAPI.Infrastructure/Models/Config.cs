using Microsoft.Extensions.Configuration;

namespace DocumentAPI.Infrastructure.Models
{
    public class Config
    {
        public string Credentials { get; set; }

        public string AdHocQueryResultsPath { get; set; }

        public string SelectIndexLookupPath { get; set; }

        public string OracleConnectionString { get; set; }

        public string S3BucketName { get; set; }

        public string S3Region { get; set; }

        public string S3AccessKeyID { get; set; }

        public string S3SecretAccessKey { get; set; }

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
                S3BucketName = config["S3BucketName"],
                S3Region = config["S3Region"],
                S3AccessKeyID = config["S3AccessKeyID"],
                S3SecretAccessKey = config["S3SecretAccessKey"],
            };
        }
    }
}
