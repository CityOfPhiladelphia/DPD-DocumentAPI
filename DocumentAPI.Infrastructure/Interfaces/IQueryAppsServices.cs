using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DocumentAPI.Infrastructure.Models;

namespace DocumentAPI.Infrastructure.Interfaces
{
    public interface IQueryAppsServices
    {
        Task<QueryAppsResult> FilterQueryAppsResultByParameters(Category category);

        Task<QueryAppsResult> GetTopResults(string categoryName);

        HttpRequestMessage BuildDocumentRequest(string categoryName, int documentId);

        Task<Stream> GetResponse(HttpRequestMessage requestMessage);
    }
}
