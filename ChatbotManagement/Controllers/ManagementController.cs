using ChatbotManagement.Common;
using ChatbotManagement.Controllers.Services.API;
using ChatbotManagement.Controllers.Services.General;
using ChatbotManagement.Models.out_VM;
using ChatbotManagement.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ChatbotManagement.Controllers
{
    [Route("[controller]")]
    [Controller]
    public class ManagementController : Controller
    {
        private readonly ManagementService _managementService;
        private readonly UserService _userService;
        private readonly Others _others;

        public ManagementController(ManagementService managementService, UserService userService, Others others)
        {
            _managementService = managementService;
            _userService = userService;
            _others = others;
        }

        /// <summary>
        /// Shows the Management view page with the data
        /// </summary>
        /// <returns>The Management view page with the data</returns>
        [HttpGet("Management")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Management()
        {
            if (_userService.ValidateToken())
            {
                ViewData["SolrConnected"] = _others.CheckSolrConnection();
                ViewData["HideNavbar"] = false;

                var filesToApprove = await _managementService.GetFilesToApprove();
                var filesAll = await _managementService.GetAll();
                ManagementTablesVM mt = new(filesToApprove!, filesAll!);

                return View(mt);
            }
            return RedirectToAction("LoginView", "Login");
        }

        /// <summary>
        /// Shows the Management view with a table showing the files that can be suggested to the user
        /// </summary>
        /// <returns>The Management view with a table showing the files that can be suggested to the user</returns>
        [HttpGet("Shown")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Shown()
        {
            if (_userService.ValidateToken())
            {
                ViewData["SolrConnected"] = _others.CheckSolrConnection();
                ViewData["HideNavbar"] = false;

                var filesToApprove = await _managementService.GetFilesToApprove();
                var filesShown = await _managementService.GetShown();
                ManagementTablesVM mt = new(filesToApprove!, filesShown!);

                return View("Management", mt);
            }
            return RedirectToAction("LoginView", "Login");
        }

        /// <summary>
        /// Shows the Management view with a table showing the files that cannot be suggested to the user
        /// </summary>
        /// <returns>The Management view with a table showing the files that cannot be suggested to the user</returns>
        [HttpGet("NotShown")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> NotShown()
        {
            if (_userService.ValidateToken())
            {
                ViewData["SolrConnected"] = _others.CheckSolrConnection();
                ViewData["HideNavbar"] = false;

                var filesToApprove = await _managementService.GetFilesToApprove();
                var filesNotShown = await _managementService.GetNotShown();
                ManagementTablesVM mt = new(filesToApprove!, filesNotShown!);

                return View("Management", mt);
            }
            return RedirectToAction("LoginView", "Login");
        }

        /// <summary>
        /// Saves the information changed on the first table to the database
        /// </summary>
        /// <param name="sf">Object with the information to save</param>
        /// <returns>The Management main view page if the authorization token is valid, else redirects to the login view page.</returns>
        [HttpPost("FirstTable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> FirstTable([FromBody] List<StoredFile2VM> sf)
        {
            ViewData["SolrConnected"] = _others.CheckSolrConnection();
            ViewData["HideNavbar"] = false;
            if (sf != null)
            {
                if (_userService.ValidateToken())
                {
                    await _managementService.PostFirstTable(sf);
                    return View("Management");
                }
                return RedirectToAction("LoginView", "Login");
            }
            return View("Management");
        }

        /// <summary>
        /// Saves the information changed on the second table to the database
        /// </summary>
        /// <param name="sf">Object with the information to save</param>
        /// <returns>The Management main view page if the authorization token is valid, else redirects to the login view page.</returns>
        [HttpPut("SecondTable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SecondTable([FromBody] List<StoredFile3VM> sf)
        {
            ViewData["SolrConnected"] = _others.CheckSolrConnection();
            ViewData["HideNavbar"] = false;
            if (sf != null)
            {
                if (_userService.ValidateToken())
                {

                    await _managementService.PostSecondTable(sf);

                    return View("Management");
                }
                return RedirectToAction("LoginView", "Login");
            }
            return View("Management");
        }

        /// <summary>
        /// Searches for the results based on the input provided
        /// </summary>
        /// <param name="solrid">Query parameter with the information to search</param>
        /// <param name="filename">Query parameter with the information to search</param>
        /// <param name="filetype">Query parameter with the information to search</param>
        /// <param name="link">Query parameter with the information to search</param>
        /// <returns>The Management page view with the search results.</returns>
        [HttpGet("Search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Search([FromQuery] string solrid="", [FromQuery] string filename="", [FromQuery] string filetype = "", [FromQuery] string link = "")
        {
            if (_userService.ValidateToken())
            {
                ViewData["SolrConnected"] = _others.CheckSolrConnection();
                ViewData["HideNavbar"] = false;

                string query = "";

                if (!solrid.IsNullOrEmpty())
                {
                    query = "solrid";
                } else if (!filename.IsNullOrEmpty())
                {
                    query = "filename";
                }
                else if (!filetype.IsNullOrEmpty())
                {
                    query = "filetype";
                }
                else if (!link.IsNullOrEmpty())
                {
                    query = "link";
                }

                // Get all files
                var filesToApprove = await _managementService.GetFilesToApprove();
                var allFiles = await _managementService.GetAll();

                // Filter the files based on the selected attribute and search query
                switch (query)
                {
                    case "solrid":
                        allFiles = allFiles!.Where(file => file.SolrId.Contains(solrid)).ToList();
                        break;
                    case "filename":
                        allFiles = allFiles!.Where(file => file.FileName.Contains(filename)).ToList();
                        break;
                    case "filetype":
                        allFiles = allFiles!.Where(file => file.FileType.Contains(filetype)).ToList();
                        break;
                    case "link":
                        allFiles = allFiles!.Where(file => file.Link.Contains(link)).ToList();
                        break;
                }
                ManagementTablesVM mt = new(filesToApprove!, allFiles!);

                return View("Management", mt);
            }
            return RedirectToAction("LoginView", "Login");
        }
    }
}
