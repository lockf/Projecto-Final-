using ChatbotManagement.Common;
using ChatbotManagement.Controllers.Services.API;
using ChatbotManagement.Models.VM;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotManagement.Controllers
{
    [Route("[controller]")]
    [Controller]
    public class LoginController : Controller
    {
        private readonly UserService _userService;
        private readonly Others _others;

        public LoginController(UserService userService, Others others)
        {
            _userService = userService;
            _others = others;
        }
        
        /// <summary>
        /// Shows the Login view page
        /// </summary>
        /// <returns>The Login view page</returns>
        [HttpGet("LoginView")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult LoginView()
        {
            ViewData["SolrConnected"] = _others.CheckSolrConnection();
            ViewData["HideNavbar"] = true;

            // Display the login view
            return View();
        }
        
        /// <summary>
        /// Shows the Create account view page
        /// </summary>
        /// <returns>The Create account view page</returns>
        [HttpGet("CreateAccountView")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateAccountView()
        {
            ViewData["SolrConnected"] = _others.CheckSolrConnection();
            ViewData["HideNavbar"] = true;

            // Display the create account view
            return View();
        }

        /// <summary>
        /// Performs user login to allow access to it's features.
        /// </summary>
        /// <param name="login">Object with the login credentials</param>
        /// <returns>To the Login view if any error on validating or executing Login occurs, if successful redirects to Dashboard main view.</returns>
        /// <remarks>
        /// POST /Todo (User)
        ///     {
        ///        "Email": "DOT@something.com",
        ///        "Password": "dotPassword",
        ///     }
        /// </remarks>
        [HttpPost("LoginManagement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginManagement([FromBody] LoginVM login)
        {
            ViewData["HideNavbar"] = true;
            List<string> erros = _userService.ValidateLogin(login);

            if (erros.Count != 0)
            {
                // Add validation errors to the model state
                foreach (var error in erros)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View("LoginView", login);
            }

            try
            {
                await _userService.Login(login);
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error while performing login.");
                return View("LoginView", login);
            }
        }

        /// <summary>
        /// It performs the user logout by deleting the login token
        /// </summary>
        /// <returns>The login view</returns>
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Logout()
        {
            ViewData["HideNavbar"] = true;
            if (_userService.ValidateToken())
            {
                _userService.Logout();
            }
            return View("LoginView");
        }
        
        /// <summary>
        /// Create account page
        /// </summary>
        /// <param name="cUser">Object with the user information.</param>
        /// <returns>Returns the Create page view if there is any error while validating the user or if it fails to add user, else returns to Login view.</returns>
        /// <remarks>
        /// POST /Todo (User)
        ///     {
        ///        "Name": "Nome aleatorio",
        ///        "Username": "NLT",
        ///        "Email": "DOT@something.com",
        ///        "Password": "dotPassword",
        ///     }
        /// </remarks>
        [HttpPost("CreateAccountManagement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAccountManagement([FromBody] CreateUser cUser)
        {
            ViewData["HideNavbar"] = true;
            List<string> erros = _userService.ValidateUser(cUser);

            if (erros.Count != 0)
            {
                // Add validation errors to the model state
                foreach (var error in erros)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View("CreateAccountView", cUser);
            }

            try
            {
                await _userService.AddUser(cUser);
                return RedirectToAction("LoginView", "Login");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error while creating the account.");
                return View("CreateAccountView", cUser);
            }
        }
    }
}
