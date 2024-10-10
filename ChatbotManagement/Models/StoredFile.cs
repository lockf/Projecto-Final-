using System.ComponentModel.DataAnnotations;

namespace ChatbotManagement.Models
{
    public class StoredFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SolrId { get; set; }

        public string? FileName { get; set; }

        [Required]
        public string FileType { get; set; }

        public string? Link { get; set; }

        [Required]
        public bool Available { get; set; }

        public StoredFile(int Id, string SolrId, string? FileName, string FileType, string? Link, bool Available = false) 
        {
            this.Id = Id;
            this.SolrId = SolrId;
            this.FileName = FileName;
            this.FileType = FileType;
            this.Link = Link;
            this.Available = Available;
            this.FileKeywords = new();
            this.Feedbacks= new();
            this.Messages = null!;
        }

        public StoredFile(string SolrId, string? FileName, string FileType, bool Available = false)
        {
            this.SolrId = SolrId;
            this.FileName = FileName;
            this.FileType = FileType;
            this.Available = Available;
            this.FileKeywords = new();
            this.Feedbacks = new();
            this.Messages = null!;
        }

        // Foreign Keys
        public virtual List<FileKeyword> FileKeywords { get; set; }
        public virtual List<Feedback> Feedbacks { get; set; }
        public virtual List<Message> Messages { get; set; }
    }
}
