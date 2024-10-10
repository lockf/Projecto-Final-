using ChatbotManagement.Controllers.Services.API;
using ChatbotManagement.Models.out_VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.RegularExpressions;

namespace ChatbotManagement.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolrController : ControllerBase
    {
        private readonly SolrService _solrService;
        private readonly MessageService _messageService;

        public SolrController(SolrService solrService, MessageService messageService)
        {
            _solrService = solrService;
            _messageService = messageService;
        }

        /// <summary>
        /// Updates Documents Info from Solr to database
        /// </summary>
        /// <returns>The request result, "Ok" if successfull or "Bad Request" if it fails</returns>
        [HttpPost("updateSolrToDb")]
        [SwaggerResponse(200, " Ok")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerOperation(Summary = "Updates Documents Info from Solr to DB", Description = "Copies the info from the documents that that exist in the Solr index but not in DB")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                // Searches for all files
                var FilesInSolr = await _solrService.Search("*:*");

                // Files not in the DB
                var FilesNotInSolr = _solrService.CheckStoredFilesToAddToDB(FilesInSolr);

                // Adds files that don't already exist in the database
                await _solrService.SolrFilesToDB(FilesInSolr);

                // Adds Keywords from files that don't already exist in the DB
                await _solrService.SolrKeywordsToDB(FilesNotInSolr);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Suggests files to the user
        /// </summary>
        /// <param name="Keywords">String with the keywords</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     GET /Todo
        ///     {
        ///        "Keywords": "Tobias, Gibbs"
        ///     }
        /// </remarks>
        /// <returns>The request result, "Ok" with the results if successfull or "Bad Request" if it fails</returns>
        [HttpGet("suggest")]
        [SwaggerResponse(200, " Ok")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerOperation(Summary = "Suggests files to the user", Description = "Suggests files to the user based on the keywords")]
        public async Task<IActionResult> Suggest([FromQuery] string Keywords)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Keywords))
                {
                    return BadRequest("No keywords.");
                }

                string[] keywords = Keywords.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (keywords.Length < 1)
                {
                    return BadRequest("No keywords.");
                }

                try
                {
                    // 1st step
                    // Searches in Solr for the Keywords
                    var FilesInSolr = await _solrService.Search(Keywords);

                    //Gets the list of the Solr Ids
                    List<string> FilesSolrIds = new();
                    foreach (var file in FilesInSolr)
                    {
                        FilesSolrIds.Add(file.SolrId);
                    }

                    // 2nd step
                    // Gets the list of stored files Ids
                    var sfIds = await _messageService.GetSuggestedFilesSolrId(FilesSolrIds);

                    // 3th step
                    // Sort based on positive feedback
                    List<StoredFileVM_out> res = _messageService.FilesSuggestionSort(sfIds);
                    return Ok(res);
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine("Error: " + ex.Message);
                return BadRequest();
            }
        }
    }
}
