namespace ChatbotManagement.Models.VM
{
    public class UserVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public UserVM(int Id, string Name, string Username, string Email)
        {
            this.Id = Id;
            this.Name = Name;
            this.Username = Username;
            this.Email = Email;
        }
    }
}
