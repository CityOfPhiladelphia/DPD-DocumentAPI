using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DocumentAPI.Infrastructure.Models;

namespace DocumentAPI.Infrastructure.Interfaces
{
    public interface IQueryAppsServices
    {
        Task<ICollection<Entity>> GetEntities();
        Task<QueryAppsResult> FilterQueryAppsResultByParameters(Category category);
        Task<QueryAppsResult> GetAllDocuments(int entityId, int categoryId);
        Task<HttpRequestMessage> StartDocumentRequest(int entityId, int categoryId, int documentId);
        Task<int> CheckExportStatus(string jobToken);
        Task<HttpRequestMessage> GetDocument(string jobToken);
        Task<bool> CheckIfDocumentIsPublic(int entityId, int categoryId, int documentId);
        Task<Stream> GetResponseStream(HttpRequestMessage requestMessage);
        Task<string> GetResponseString(HttpRequestMessage requestMessage);
    }
}
