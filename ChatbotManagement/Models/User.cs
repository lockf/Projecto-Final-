using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatbotManagement.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public User(int Id, string Name, string Username, string Email, string Password)
        {
            this.Id = Id;
            this.Name = Name;
            this.Username = Username;
            this.Email = Email;
            this.Password = Password;
        }
        public User(int Id, string Name, string Username, string Email)
        {
            this.Id = Id;
            this.Name = Name;
            this.Username = Username;
            this.Email = Email;
        }
        public User(string Name, string Username, string Email, string Password)
        {
            this.Name = Name;
            this.Username = Username;
            this.Email = Email;
            this.Password = Password;
        }
        public User(string Name, string Username, string Email)
        {
            this.Name = Name;
            this.Username = Username;
            this.Email = Email;
        }
    }
}
