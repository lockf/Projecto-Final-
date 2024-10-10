namespace ChatbotManagement.Models.VM
{
    public class CreateUser
    {
        public string Name { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public CreateUser(string Name, string Username, string Email, string Password)
        {
            this.Name = Name;
            this.Username = Username;
            this.Email = Email;
            this.Password = Password;
        }
    }
}
