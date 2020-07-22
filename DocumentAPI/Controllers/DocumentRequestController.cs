using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DocumentAPI.Infrastructure.Interfaces;
using DocumentAPI.Infrastructure.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace DocumentAPI.Controllers
{
    [Route("api/v1/document-request")]
    [ApiController]
    public class DocumentRequestController : ControllerBase
    {
        private readonly IHealthCheckServices _healthCheckServices;
        private readonly IQueryAppsServices _queryAppsServices;
        private ILogger _logger;

        public DocumentRequestController(IHealthCheckServices healthCheckServices, IQueryAppsServices queryAppsServices, ILogger<DocumentRequestController> logger)
        {
            _healthCheckServices = healthCheckServices;
            _queryAppsServices = queryAppsServices;
            _logger = logger;
        }

        // GET: api/v1/health-check
        /// <summary>
        /// Result of this endpoint will indicate whether API is functioning normally
        /// </summary>
        /// <returns>Boolean</returns>

        [HttpGet("health-check")]
        public async Task<IActionResult> HealthCheck()
        {
            var healthCheckResponse = new HealthCheckResponse();

            if (_queryAppsServices != null)
            {
                var internalHealthCheck = _healthCheckServices.CheckInternalDependencies();

                //var externalHealthCheck = await _healthCheckServices.CheckExternalDependencies();
                //healthCheckResponse.Results.Add(
                //    new HealthCheckResult()
                //    {
                //        Success = externalHealthCheck.Success,
                //        Message = externalHealthCheck.Message
                //    });

                await Task.WhenAll(new[] { internalHealthCheck });
                healthCheckResponse.Results.AddRange(new[] {
                    new HealthCheckResult()
                    {
                        Success = internalHealthCheck.Result.Success,
                        Message = internalHealthCheck.Result.Message
                    }
                });

                healthCheckResponse.Success = healthCheckResponse.Results.All(i => i.Success);
            }
            else
            {
                healthCheckResponse.Success = false;
            }

            healthCheckResponse.Message = string.Join(Environment.NewLine, healthCheckResponse.Results.Select(i => i.Message));
            var response = JsonConvert.SerializeObject(new { healthCheckResponse.Success, healthCheckResponse.Message }, Formatting.Indented);

            if (healthCheckResponse.Success)
            {
                return new JsonResult(response);
            }
            else
            {

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // GET: api/v1/document-request/entities
        /// <summary>
        /// Get the initial list of Entities. Each entity has a distinct set of Categories.
        /// </summary>
        /// <returns>Json(List - Entity)</returns>
        [HttpGet("entities")]
        public async Task<IActionResult> GetEntities()
        {
            var entities = await _queryAppsServices.GetEntities();
            if (entities != null && entities.Any())
            {
                _logger.LogInformation("Successfully retrieved entity list.");
                return new JsonResult(entities);
            }
            else
            {
                _logger.LogError("Cannot get entities.");
                return NotFound();
            }
        }

        // GET: api/v1/document-request/document-categories/{entityName}
        /// <summary>
        /// Get list of Document Categories available for a given Entity. Each document category is mapped to a Repository in the ApplicationXTender.
        /// </summary>
        /// <param name="entityName">The Name property of the selected Entity</param>
        /// <returns>Json(List - Category)</returns>
        [HttpGet("document-categories/{entityName}")]
        public async Task<IActionResult> GetDocumentCategories(string entityName)
        {
            var entities = await _queryAppsServices.GetEntities();
            var documentCategories = entities.SingleOrDefault(i => i.Name == entityName)?.Categories;
            if (documentCategories != null)
            {
                _logger.LogInformation("Successfully retrieved document category list.");
                return new JsonResult(documentCategories);
            }
            else
            {
                _logger.LogError("Cannot get document categories.");
                return NotFound();
            }
        }

        // GET: api/v1/document-request/document-list/{entityName}/{categoryName}
        /// <summary>
        /// Pass the Entity Name and Category Name to retrieve a full list of documents and their attributes for that Category.
        /// </summary>
        /// <param name="entityName">The Name property of the selected Entity</param>
        /// <param name="categoryName">The Name property of the selected Category</param>
        /// <returns>Json(QueryAppsResult)</returns>
        [HttpGet("document-list/{entityName}/{categoryName}")]
        public async Task<IActionResult> GetDocumentList(string entityName, string categoryName)
        {
            var entities = await _queryAppsServices.GetEntities();
            var entity = entities.SingleOrDefault(i => i.Name == entityName);
            var category = entity?.Categories.SingleOrDefault(i => i.Name == categoryName);
            if (category != null)
            {
                _logger.LogInformation("Successfully retrieved document category.");
                var queryAppsResult = await _queryAppsServices.GetAllDocuments(entity.Id.GetValueOrDefault(), category.Id.GetValueOrDefault());
                if (queryAppsResult.Entries.Count > 0)
                {
                    _logger.LogInformation("Successfully retrieved document category & document list.");
                    return new JsonResult(queryAppsResult.ToApiResult());
                }
                else
                {
                    _logger.LogWarning("Successfully retrieved document category, but no documents were found.");
                    return NotFound();
                }
            }
            else
            {
                _logger.LogError("Cannot get document category.");
                return NotFound();
            }
        }

        // POST: api/v1/document-request/filtered-document-list
        /// <summary>
        /// Pass a Category object with filters applied in the body of the request to retrieve a filtered list of documents and their attributes.
        /// </summary>
        /// <param name="category">A category object</param>
        /// <returns>Json(QueryAppsResult)</returns>
        [HttpPost("filtered-document-list")]
        public async Task<IActionResult> GetFilteredDocumentList([FromBody]Category category)
        {
            var xTenderDocumentList = await _queryAppsServices.FilterQueryAppsResultByParameters(category);

            if (xTenderDocumentList.Entries != null)
            {
                _logger.LogInformation("Successfully retrieved document category & filtered document list.");
                return new JsonResult(xTenderDocumentList.ToApiResult());
            }
            else
            {
                _logger.LogWarning("Successfully retrieved document category, but no documents were found.");
                return NotFound();
            }
        }

        // GET: api/v1/document-request/get-document/{categoryName}/{documentId}
        /// <summary>
        /// Pass in the category name and document id, and receive back the PDF represented by that id from the ApplicationXTender Repository.
        /// </summary>
        /// <param name="entityName">The Name property of the category object's Entity, which represents the Entity (Organization) which the Category belongs to</param>
        /// <param name="categoryName">The Name property of the category object, which represents the name of the Category as a repository in ApplicationXTender</param>
        /// <param name="documentId">The Id that uniquely identifies the document in the ApplicationXTender repository</param>
        /// <returns>FileStream(PDF)</returns>
        [HttpGet("get-document/{entityName}/{categoryName}/{documentId}")]
        [Produces("application/pdf", "application/problem+json")]
        public async Task<IActionResult> GetDocument(string entityName, string categoryName, int documentId)
        {
            var entities = await _queryAppsServices.GetEntities();
            var entity = entities.SingleOrDefault(i => i.Name == entityName);
            if (entity != null)
            {
                var category = entity.Categories.SingleOrDefault(i => i.Name == categoryName);
                if (category != null)
                {
                    var isPublic = _queryAppsServices.CheckIfDocumentIsPublic(entity.Id.GetValueOrDefault(), category.Id.GetValueOrDefault(), documentId);

                    if (!await isPublic)
                    {
                        return NotFound();
                    };

                    var request = await _queryAppsServices.BuildDocumentRequest(entity.Id.GetValueOrDefault(), category.Id.GetValueOrDefault(), documentId);

                    var file = await _queryAppsServices.GetResponse(request);
                    return new FileStreamResult(file, "application/pdf");
                }
                else
                {
                    _logger.LogError($"Category: {categoryName} not found.");
                    return NotFound();
                }
            }
            else
            {
                _logger.LogError($"Entity: {entityName} not found.");
                return NotFound();
            }
        }
    }
}