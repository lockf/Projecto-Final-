namespace ChatbotManagement.Models.VM
{
    /// <summary>
    /// View model class for token response
    /// </summary>
    public class TokenVM
    {
        public string Token { get; set; }

        public string Username { get; set; }

        /// <summary>
        /// Class' constructor method
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="username">Username</param>
        public TokenVM(string token, string username)
        {
            this.Token = token;
            this.Username = username;
        }
    }
}
