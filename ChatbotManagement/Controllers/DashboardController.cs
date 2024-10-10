using ChatbotManagement.Common;
using ChatbotManagement.Controllers.Services.API;
using ChatbotManagement.Controllers.Services.General;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotManagement.Controllers
{
    [Route("[controller]")]
    [Controller]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly SolrService _solrService;
        private readonly UserService _userService;
        private readonly Others _others;

        public DashboardController(DashboardService dashboardService, SolrService solrService, UserService userService, Others others)
        {
            _dashboardService = dashboardService;
            _solrService = solrService;
            _userService = userService;
            _others = others;
        }

        /// <summary>
        /// Dashboard main page
        /// </summary>
        /// <returns>The view of the Dashboard main page if successful or redirects to Login page if it fails to validate the token.</returns>
        [HttpGet("Index")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Index()
        {
            if (_userService.ValidateToken())
            {
                ViewData["SolrConnected"] = _others.CheckSolrConnection();
                ViewData["HideNavbar"] = false;

                var dash = await _dashboardService.GetDashboardInfo();
                return View(dash);
            }
            else
            {
                return RedirectToAction("LoginView", "Login");
            }
        }

        /// <summary>
        /// Updates database with the data from the Solr server
        /// </summary>
        /// <returns>Dashboard main view or login view if the token validation fails.</returns>
        [HttpPost("UpdateDB")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateDB()
        {
            ViewData["HideNavbar"] = false;
            if (_userService.ValidateToken())
            {
                // Searches for all files
                var FilesInSolr = await _solrService.Search("*:*");

                // Files not in the DB
                var FilesNotInSolr = _solrService.CheckStoredFilesToAddToDB(FilesInSolr);

                // Adds files that don't already exist in the database
                await _solrService.SolrFilesToDB(FilesInSolr);

                // Adds Keywords from files that don't already exist in the DB
                await _solrService.SolrKeywordsToDB(FilesNotInSolr);

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("LoginView", "Login");
            }
        }
    }
}
