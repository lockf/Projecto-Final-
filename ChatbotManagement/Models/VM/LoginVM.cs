using System.Text.Json.Serialization;

namespace ChatbotManagement.Models.VM
{
    /// <summary>
    /// View model class for login request
    /// </summary>
    public class LoginVM
    {
        public string Email { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Class' constructor method
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        [JsonConstructor]
        public LoginVM(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }
    }
}
