namespace ChatbotManagement.Models.VM
{
    public class UserInfo
    {
        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public UserInfo(string Name, string Username, string Password)
        {
            this.Name = Name;
            this.Username = Username;
            this.Password = Password;
        }
    }
}
