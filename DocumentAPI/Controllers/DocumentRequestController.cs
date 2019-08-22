using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DocumentAPI.Infrastructure.Interfaces;
using DocumentAPI.Infrastructure.Models;

namespace DocumentAPI.Controllers
{
    [Route("api/v1/document-request")]
    [ApiController]
    public class DocumentRequestController : ControllerBase
    {
        private readonly IQueryAppsServices _queryAppsServices;

        public DocumentRequestController(IQueryAppsServices queryAppsServices)
        {
            _queryAppsServices = queryAppsServices;
        }

        // GET: api/v1/document-request/document-categories
        /// <summary>
        /// Get the initial list of Document Categories. Each document category is mapped to a Repository in the ApplicationXTender.
        /// </summary>
        /// <returns>Json(List - Category)</returns>
        [HttpGet("document-categories")]
        public JsonResult GetDocumentCategories()
        {
            return new JsonResult(DocumentCategories.Categories);
        }

        // POST: api/v1/document-request/filtered-document-list
        /// <summary>
        /// Pass a Category object with filters applied in the body of the request to retrieve a filtered list of documents and their attributes.
        /// </summary>
        /// <param name="category">A category object</param>
        /// <returns>Json(QueryAppsResult)</returns>
        [HttpPost("filtered-document-list")]
        public async Task<JsonResult> GetFilteredDocumentList([FromBody]Category category)
        {
            var xTenderDocumentList = await _queryAppsServices.FilterQueryAppsResultByParameters(category);

            if (xTenderDocumentList.Entries != null)
            {
                return new JsonResult(xTenderDocumentList.ToApiResult());
            }
            else
            {
                return new JsonResult(NotFound());
            }
        }

        // GET: api/v1/document-request/get-document/{categoryName}/{documentId}
        /// <summary>
        /// Pass in the category name and document id, and receive back the PDF represented by that id from the ApplicationXTender Repository.
        /// </summary>
        /// <param name="categoryName">The Name property of the category object, which represents the name of the repository in ApplicationXTender</param>
        /// <param name="documentId">The Id that uniquely identifies the document in the ApplicationXTender repository</param>
        /// <returns>FileStream(PDF)</returns>
        [HttpGet("get-document/{categoryName}/{documentId}")]
        [Produces("application/pdf", "application/problem+json")]
        public async Task<IActionResult> GetDocument(string categoryName, int documentId)
        {
            var isPublic = _queryAppsServices.CheckIfDocumentIsPublic(categoryName, documentId);

            if (!await isPublic)
            {
                return NotFound();
            };

            var request = _queryAppsServices.BuildDocumentRequest(categoryName, documentId);

            var file = await _queryAppsServices.GetResponse(request);
            return new FileStreamResult(file, "application/pdf");
        }
    }
}