using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ChatbotManagement.Models.VM;
using ChatbotManagement.Models;
using ChatbotManagement.Common;
using System.Net.Mail;
using CheckPasswordStrength;

namespace ChatbotManagement.Controllers.Services.API
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IConfiguration configuration, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the JWT secret key from the "appsettings.json"
        /// </summary>
        /// <returns>The string if it finds it, otherwise the key used is simpler.</returns>
        private string GetJwtKey()
        {
            var jwt = _configuration["Jwt:Key"];
            if (jwt != null)
            {
                return jwt;
            }
            else { return "EMPTY"; }
        }

        /// <summary>
        /// API login to get authentication token
        /// </summary>
        /// <param name="login">LoginVM with authentication credentials</param>
        /// <returns>Tuple with the TokenVM, string, int if successful or null otherwise.</returns>
        public async Task<Tuple<TokenVM, string, int>?> Login(LoginVM login)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == login.Email);

            if (user != null)
            {
                bool isValidPassword = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);
                if (isValidPassword)
                {
                    var token = GenerateToken(user.Name, "User");

                    // Store token in a cookie
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddHours(12) // Defines cookie validity (lifetime)
                    };
                    _httpContextAccessor.HttpContext!.Response.Cookies.Append("AuthorizationToken", token, cookieOptions);

                    return new Tuple<TokenVM, string, int>(new TokenVM(GenerateToken(user.Name, "User"), user.Name), "User", user.Id);
                }
                throw new Exception("User Null");
            }
            throw new Exception("Error in Login");
        }

        /// <summary>
        /// Erases the login token from the cookie.
        /// </summary>
        public void Logout()
        {
            // Set the cookie options with an expired date
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddYears(-1) // Set expiration date to a past date
            };
            _httpContextAccessor.HttpContext!.Response.Cookies.Delete("AuthorizationToken", cookieOptions);
        }

        /// <summary>
        /// Create User in db
        /// </summary>
        /// <param name="user">User info to be added to the db</param>
        public async Task AddUser(CreateUser user)
        {
            var _user = new User(user.Name, user.Username, user.Email, BCrypt.Net.BCrypt.HashPassword(user.Password));
            _context.Users.Add(_user);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a user from the list of users
        /// </summary>
        /// <param name="email">Email of the user that is to be removed"</param>
        public async Task RemoveUser(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(e => e.Email.Equals(email));
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verify if login is valid for the User
        /// </summary>
        /// <param name="login">LoginVM to validate</param>
        /// <returns>A List of strings object with errors , or an empty on if there aren't any errors</returns>
        public List<string> ValidateLogin(LoginVM login)
        {
            List<string> errors = new();

            if (!UserExists(login.Email))
            {
                errors.Add("There is no User with this email");
            }
            else if (!IsEmailValid(login.Email))
            {
                errors.Add("Invalid Email");
            }
            else if (login.Password.Trim().Equals(""))
            {
                errors.Add("Invalid Password");
            }
            return errors;
        }

        /// <summary>
        /// Verify if info is valid to create user in the DB
        /// </summary>
        /// <param name="user">User parameters to validate</param>
        /// <returns>A list of string with errrors, or an empty list if there aren't any errors</returns>
        public List<string> ValidateUser(CreateUser user)
        {
            List<string> errors = new();
            string pass = user.Password.PasswordStrength().Value;

            if (user.Name.Trim().Equals("") || user.Name.Trim().Length < 3)
            {
                errors.Add("Invalid Name. Either empty field or not enough characters.");
            }
            else if (user.Username.Trim().Equals("") || user.Username.Trim().Length < 3)
            {
                errors.Add("Invalid UserName. Either empty field or not enough characters.");
            }
            else if (!IsEmailValid(user.Email))
            {
                errors.Add("Email invalid");
            }
            else if (UserExists(user.Email))
            {
                errors.Add("Email already exists");
            }
            else if (pass.Equals("Weak") || pass.Equals("Medium"))
            {
                errors.Add("Password strength is " + pass);
            }

            return new List<string>(errors);
        }

        /// <summary>
        /// Verify if the email is valid
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>Return true if email is valid;
        /// Return false if email isn't valid;</returns>
        public bool IsEmailValid(string email)
        {
            try
            {
                MailAddress m = new(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets Users list
        /// </summary>
        /// <returns>Return list of users and their info</returns>
        public async Task<List<UserVM>> GetUsersListAndInfo()
        {
            if (UserExists())
            {
                var users = await _context.Users.ToListAsync();
                List<UserVM> userList = new();

                foreach (var user in users)
                {
                    UserVM tempUser = new(user.Id, user.Name, user.Username, user.Email);
                    userList.Add(tempUser);
                }

                return userList;
            }
            throw new Exception("Empty database table");
        }

        /// <summary>
        /// Gets user info
        /// </summary>
        /// <param name="userId">Id of the User</param>
        public async Task<UserVM> UserInfo(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                UserVM userInfo = new(user.Id, user.Name, user.Username, user.Email);
                return userInfo;
            }
            throw new Exception("Empty database table");
        }

        /// <summary>
        /// Verifies if the user exists in DB
        /// </summary>
        /// <param name="email">Id of the admin that is to be verified if exists"</param>
        public bool UserExists(string email)
        {
            return _context.Users.Any(e => email.Equals(e.Email));
        }

        /// <summary>
        /// Verifies if exists any user in DB
        /// </summary>
        public bool UserExists()
        {
            return _context.Users.Any();
        }

        /// <summary>
        /// Edit User info
        /// </summary>
        /// <param name="userInfo">Information to be altered</param>
        /// <param name="userId">Id of the User to change info</param>
        public async Task EditUserInfo(UserInfo userInfo, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            string pass = userInfo.Password.PasswordStrength().Value;

            if (user != null)
            {
                if (!userInfo.Name.Trim().Equals("") || userInfo.Name.Trim().Length < 3)
                {
                    user.Name = userInfo.Name;
                }
                if (!userInfo.Username.Trim().Equals("") || userInfo.Username.Trim().Length < 3)
                {
                    user.Username = userInfo.Username;
                }
                if (!userInfo.Password.Trim().Equals("") || pass.Equals("Strong"))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(userInfo.Password);
                }
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verify if info is valid to create user in the DB
        /// </summary>
        /// <param name="userInfo">User parameters to validate</param>
        /// <returns>A list of string with errrors, or an empty list if there aren't any errors</returns>
        public List<string> ValidateUserNewInfo(UserInfo userInfo)
        {
            List<string> errors = new();
            string pass = userInfo.Password.PasswordStrength().Value;

            if (!userInfo.Name.Trim().Equals("") || userInfo.Name.Trim().Length < 3)
            {
                errors.Add("Invalid Name. Not enough characters.");
            }
            else if (!userInfo.Username.Trim().Equals("") || userInfo.Username.Trim().Length < 3)
            {
                errors.Add("Invalid UserName. Not enough characters.");
            }
            else if (!userInfo.Password.Trim().Equals("") || !pass.Equals("Strong"))
            {
                errors.Add("Password strength is " + pass);
            }

            return new List<string>(errors);
        }

        /// <summary>
        /// Generate JW Token
        /// </summary>
        /// <param name="username"> Username to encrypt</param>
        /// <param name="role">Role to encrypt</param>
        /// <returns>String with Token</returns>
        private string GenerateToken(string username, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken
            (
                issuer: _configuration.GetValue<string>("Jwt:Issuer"),
                audience: _configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtKey())),
                    SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        /// <summary>
        /// Validates token from cookie (should it exist)
        /// </summary>
        /// <param name="lifetime">Indicates if the token is for lifetime, it is false by default</param>
        /// <returns>False if there is no token or is invalid, True if the token is valid</returns>
        public bool ValidateToken(bool lifetime = false)
        {
            // "AuthorizationToken" is the Cookie name used when setting the token
            var token = _httpContextAccessor.HttpContext!.Request.Cookies["AuthorizationToken"];

            if (string.IsNullOrEmpty(token))
            {
                // Token not found in cookies
                return false;
            }

            var jwtSecretKey = GetJwtKey();

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = lifetime,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
                ClockSkew = TimeSpan.Zero // Set to TimeSpan.Zero to ensure token expiration is strictly enforced
            };

            try
            {
                // Token is valid, and claims can be accessed
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var username = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;

                return true;
            }
            catch (Exception ex)
            {
                // Token validation failed, token is either invalid or expired
                Console.WriteLine("Token validation failed. Exception message: " + ex.Message);
                return false;
            }

        }
    }
}
