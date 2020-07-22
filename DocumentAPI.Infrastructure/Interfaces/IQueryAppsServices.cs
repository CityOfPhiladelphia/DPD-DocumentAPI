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
        Task<HttpRequestMessage> BuildDocumentRequest(int entityId, int categoryId, int documentId);
        Task<bool> CheckIfDocumentIsPublic(int entityId, int categoryId, int documentId);
        Task<Stream> GetResponse(HttpRequestMessage requestMessage);
    }
}
