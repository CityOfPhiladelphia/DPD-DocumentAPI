using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentAPI.Infrastructure.Models;
using Attribute = DocumentAPI.Infrastructure.Models.Attribute;

namespace DocumentAPI.Infrastructure.Interfaces
{
    public interface IQueryAppsServices
    {
        Task<QueryAppsResult> FilterQueryAppsResultByParameters(QueryAppsResult xTenderDocumentList, Category category);

        Task<QueryAppsResult> RequestDocuments(string categoryName);

        Task<Stream> GetResponse(HttpRequestMessage requestMessage);
    }
}
