
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RestSharp;
using Smart_Document_Management_System.Helpers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Structures;
namespace Smart_Document_Management_System.Controllers
{

    [Route("[controller]")]
    [ApiController]

    public class DocumentController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            HttpStatusCode _HttpStatusCode = HttpStatusCode.OK;
            (string summary, string category, string reasoning) = ("", "", "");
            Gen_Response response = new Gen_Response();
            try
            {
                StringBuilder sb = new StringBuilder();
                var request = HttpContext.Request;
                CommonMethods.DebugLog($"UPLOADDOCUMENT_REQUEST|{request.GetDisplayUrl}");
                if (file == null || file.Length == 0)
                {
                    CommonMethods.DebugLog($"UPLOADDOCUMENT_FAILURE|{HttpStatusCode.NoContent}|No File");

                    _HttpStatusCode = HttpStatusCode.NoContent;
                    response.Message = "No file uploaded.";
                }
                else if (CommonMethods.ExistInDB(file.FileName))
                {
                    _HttpStatusCode = HttpStatusCode.BadRequest;
                    response.Message = "File already exists";
                }
                else
                {

                    string guid = Guid.NewGuid().ToString();

                    var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }


                    //var summary = await GenerateSummaryAsync(text);
                    //var category = await CategorizeAsync(text);
                    if (CommonMethods.ExtractTextFrom(filePath, out string text))
                    {
                        GemmaProcessor processor = new GemmaProcessor(CommonMethods.GetFromConfig("OTHER", "API-KEY"));

                        (summary, category, reasoning) = await processor.ProcessFileAsync(filePath, text, guid);
                        // Get embedding
                        var embedding = await processor.GetEmbeddingAsync(text);
                        string embeddingJson = System.Text.Json.JsonSerializer.Serialize(embedding);

                        // Store in DB (update your DBUpdateQuery to accept embeddingJson)
                        CommonMethods.DBUpdateQuery(guid, file.FileName, summary, category, reasoning, text, embeddingJson);

                        CommonMethods.DebugLog($"UPLOADDOCUMENT_SUCCESS|{guid}|{HttpStatusCode.OK}|{filePath}");
                        response.Message = "File uploaded and processed successfully.";
                    }
                    else {
                        _HttpStatusCode = HttpStatusCode.NotAcceptable;
                        response.Message = "No text extracted from the document.";
                        CommonMethods.DebugLog($"UPLOADDOCUMENT_FAILURE|{guid}|{_HttpStatusCode}|No Text Extracted");
                    }
                }
                response.statusCode = _HttpStatusCode;
                response.response = new TextResponse { Summary = summary, Category = category, Reasoning = reasoning };
                
                return StatusCode((int)_HttpStatusCode,response);
            }
            catch (Exception ex)
            {
                _HttpStatusCode = HttpStatusCode.InternalServerError;
                response.statusCode = _HttpStatusCode;
                response.Message = "An error occurred while processing the file.";
                CommonMethods.ExceptionLog(ex, nameof(UploadDocument));
                return StatusCode((int)_HttpStatusCode, response);
            }
        }
        [HttpGet("get")]
        public OkObjectResult Get()
        {
            // This method can be used to return a list of documents or other information
            // For now, it does nothing
            return Ok("Document management system is running.");
        }
        [HttpGet("list")]
        public async Task<IActionResult> ListDocuments()
        {
                var documents = new List<DocumentWithEmbedding>();
            using (var conn = new MySqlConnection(CommonMethods.DB))
            {
                string sql = $"SELECT ID, FileName, Summary, Category, Reasoning, Content, Embedding AS EmbeddingJson FROM {CommonMethods.Table}";
                var dbDocs = conn.Query<DocumentWithEmbedding>(sql);
                documents = dbDocs.ToList();
            }
            
            return Ok(documents.Select(v=>new { ID = v.ID,FileName = v.FileName, Category = v.Category, Summary = v.Summary }));
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteDocument([FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File Name is required.");
            }
            using (var conn = new MySqlConnection(CommonMethods.DB))
            {
                string sql = $"DELETE FROM {CommonMethods.Table} WHERE FileName = @FileName";
                var result = await conn.ExecuteAsync(sql, new { FileName = fileName });
                if (result > 0)
                {
                    return Ok("Document deleted successfully.");
                }
                else
                {
                    return NotFound("Document not found.");
                }
            }
        }
        [HttpGet("semantic-search")]
        public async Task<IActionResult> SemanticSearch([FromQuery] string query)
        {
            // 1. Get embedding for the query
            GemmaProcessor processor = new GemmaProcessor(CommonMethods.GetFromConfig("OTHER", "API-KEY"));

            var queryEmbedding = await processor.GetEmbeddingAsync(query);
            if (queryEmbedding == null)
                return BadRequest("Failed to generate embedding for query.");

            // 2. Fetch all documents from MySQL
            var documents = new List<DocumentWithEmbedding>();
            using (var conn = new MySqlConnection(CommonMethods.DB))
            {
                string sql = $"SELECT ID, FileName, Summary, Category, Reasoning, Content, Embedding AS EmbeddingJson FROM {CommonMethods.Table}";
                var dbDocs = conn.Query<DocumentWithEmbedding>(sql);
                documents = dbDocs.ToList();
            }

            // 3. Compute cosine similarity for each document
            var results = documents
                .Select(doc => new
                {
                    Document = doc,
                    Score = CommonMethods.CosineSimilarity(queryEmbedding, doc.Embedding)
                })
                .OrderByDescending(x => x.Score)
                .Take(10)
                .ToList();

            return Ok(results);
        }

    }
}