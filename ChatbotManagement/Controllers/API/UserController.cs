using ChatbotManagement.Controllers.Services.API;
using ChatbotManagement.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ChatbotManagement.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets a list of Users
        /// </summary>
        /// <returns>A list of Users</returns>
        [HttpGet("getUsers")]
        [SwaggerResponse(200, " Ok")]
        [SwaggerResponse(404, "Not Found")]
        [SwaggerOperation(Summary = "List of Users", Description = "Lists of Users in DB")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUsers()
        {
            bool existsU = _userService.UserExists();
            if (existsU)
            {
                List<UserVM> list = await _userService.GetUsersListAndInfo();
                return Ok(list);
            }
            return NotFound();
        }

        /// <summary>
        /// Gets the User info
        /// </summary>
        /// <returns>A User information</returns>
        [HttpGet("{userId}/getUserInfo")]
        [SwaggerResponse(200, " Ok")]
        [SwaggerResponse(404, "Not Found")]
        [SwaggerOperation(Summary = "User info", Description = "Return info about the User")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserInfo(int userId)
        {
            bool existsA = _userService.UserExists();
            if (existsA)
            {
                UserVM user = await _userService.UserInfo(userId);
                return Ok(user);
            }
            return NotFound();
        }

        /// <summary>
        /// Creates User in DB.
        /// </summary>
        /// <param name="user"></param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /Todo (User)
        ///     {
        ///        "Name": "Tobias Gibbs",
        ///        "Username": "toby",
        ///        "Email": "toby@domain.com",
        ///        "Password": "da9s84fd"
        ///     }
        /// </remarks>
        /// <returns>The request result, "Ok" if successfull or "Bad Request" if it fails</returns>
        [HttpPost("createUser")]
        [SwaggerResponse(200, " Ok")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerOperation(Summary = "Create User", Description = "Create User on DataBase")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUser user)
        {
            List<string> erros = _userService.ValidateUser(user);

            if (erros.Count != 0)
            {
                return BadRequest(erros);
            }

            try
            {
                await _userService.AddUser(user);
                return Ok("Teste aceite");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Changes User info.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="userId"></param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     PUT /Todo (Participante)
        ///     {
        ///        "Name": "Tobias Gibbs",
        ///        "Username": "toby",
        ///        "Password": "da9s84fd"
        ///     }
        /// </remarks>
        /// <returns>The request result, "Ok" if successfull or "Bad Request" if it fails</returns>
        [HttpPut("{userId}/changeUserInfo")]
        [SwaggerResponse(200, " Ok")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerOperation(Summary = "Edit User info", Description = "Change User info on DataBase")]
        [Authorize(Roles = "User,SuperAdmin")]
        public async Task<IActionResult> EditUser([FromBody] UserInfo userInfo, int userId)
        {
            List<string> erros = _userService.ValidateUserNewInfo(userInfo);

            if (erros.Count != 0)
            {
                return BadRequest(erros);
            }

            try
            {
                await _userService.EditUserInfo(userInfo, userId);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Deletes User.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     "toby@domain.com"
        /// </remarks>
        /// <param name="email"></param>
        /// <returns>The request result, "Ok" if successfull or "Bad Request" if it fails</returns>
        [HttpDelete("removeUser")]
        [SwaggerResponse(200, " Ok")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerOperation(Summary = "Delete User", Description = "Deletes User of the DataBase")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteUser([FromBody] string email)
        {
            List<string> erros = new();

            if (!_userService.UserExists(email))
            {
                erros.Add("No User exists with this email");
            }
            if (erros.Count != 0)
            {
                return BadRequest(erros);
            }

            try
            {
                await _userService.RemoveUser(email);
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }

        /// <summary>
        /// Login for Users
        /// </summary>
        /// <param name="login"></param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /Todo (User)
        ///     {
        ///        "Email": "DOT@something.com",
        ///        "Password": "dotPassword",
        ///     }
        /// </remarks>
        /// <returns>The request result, "Ok" if successfull or "Bad Request" if it fails</returns>
        [HttpPost("Login")]
        [SwaggerResponse(200, " Ok")]
        [SwaggerResponse(400, "Bad Request")]
        [SwaggerResponse(404, "Not Found")]
        [SwaggerOperation(Summary = "Login", Description = "User Login")]
        public async Task<IActionResult> PostLogin([FromBody] LoginVM login)
        {
            List<string> erros = _userService.ValidateLogin(login);

            if (erros.Count != 0)
            {
                return BadRequest(erros);
            }

            var log = await _userService.Login(login);
            if (log == null)
            {
                erros.Add("Password incorreta");
                return BadRequest(erros);
            }
            return Ok(log);
        }
    }
}
