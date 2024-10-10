using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatbotManagement.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string ChannelId { get; set; }

        [Required]
        public string ConversationId { get; set; }

        public Message(string Text, DateTime Date, string ChannelId, string ConversationId, int? StoredFileId)
        {
            this.Text = Text;
            this.Date = Date;
            this.ChannelId = ChannelId;
            this.ConversationId = ConversationId;
            this.StoredFileId = StoredFileId;
            this.StoredFile = null!;
        }

        // Foreign Keys

        [ForeignKey("StoredFileId")]
        public int? StoredFileId { get; set; }
        public virtual StoredFile StoredFile { get; set; }
    }
}
