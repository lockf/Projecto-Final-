using System.ComponentModel.DataAnnotations;

namespace ChatbotManagement.Models
{
    public class Keyword
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Word { get; set; }

        public Keyword(int Id, string Word) 
        {
            this.Id= Id;
            this.Word = Word;
            this.FileKeywords = new();
        }

        public Keyword(string Word)
        {
            this.Word = Word;
        }

        // Foreign Keys

        public virtual List<FileKeyword> FileKeywords { get; set; }
    }
}
