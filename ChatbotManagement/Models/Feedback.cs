using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatbotManagement.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Opinion { get; set; }

        public string Conclusion { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Id">Id of the feedback</param>
        /// <param name="Opinion">Opinion of the user</param>
        /// <param name="Conclusion">Conclusion of the opinion, Positive, Neutral or Negative, determined by machine learning</param>
        /// <param name="StoredFileId">Id of the file to which the opinion belongs to</param>
        public Feedback(int Id, string Opinion, string Conclusion, int StoredFileId) 
        {
            this.Id = Id;
            this.Opinion = Opinion;
            this.Conclusion = Conclusion;
            this.StoredFileId = StoredFileId;
            this.StoredFile = null!;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Opinion">Opinion of the user</param>
        /// <param name="Conclusion">Conclusion of the opinion, Positive, Neutral or Negative, determined by machine learning</param>
        /// <param name="StoredFileId">Id of the file to which the opinion belongs to</param>
        public Feedback(string Opinion, string Conclusion, int StoredFileId)
        {
            this.Opinion = Opinion;
            this.Conclusion = Conclusion;
            this.StoredFileId = StoredFileId;
            this.StoredFile = null!;
        }

        // Foreign Keys

        [ForeignKey("StoredFileId")]
        public int StoredFileId { get; set; }
        public virtual StoredFile StoredFile { get; set; }
    }
}
