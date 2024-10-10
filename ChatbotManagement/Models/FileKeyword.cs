using System.ComponentModel.DataAnnotations.Schema;

namespace ChatbotManagement.Models
{
    public class FileKeyword
    {
        public FileKeyword(int KeywordId, int StoredFileId)
        {
            this.KeywordId = KeywordId;
            this.Keyword = null!;
            this.StoredFileId = StoredFileId;
            this.StoredFile = null!;
        }

        // Foreign Keys

        [ForeignKey("KeywordId")]
        public int KeywordId { get; set; }
        public virtual Keyword Keyword { get; set; }

        [ForeignKey("StoredFileId")]
        public int StoredFileId { get; set; }
        public virtual StoredFile StoredFile { get; set; }
    }
}
