using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ChatbotManagement.Models.out_VM
{
    [NotMapped]
    public class DashboardVM
    {
        public int TotalKeywords { get; set; }
        public int TotalFiles { get; set; }
        public int TotalFeedback { get; set; }
        public int TotalMessages { get; set; }
        public int NegativeFeedback { get; set; }
        public int NeutralFeedback { get; set; }
        public int PositiveFeedback { get; set; }
        public decimal NegativeFeedbackPer { get; set; }
        public decimal NeutralFeedbackPer { get; set; }
        public decimal PositiveFeedbackPer { get; set; }
        public int FilesNotShown { get; set; }
        public int FilesShown { get; set; }
        public decimal FilesNotShownPer { get; set; }
        public decimal FilesShownPer { get; set; }

        [JsonConstructor]
        public DashboardVM(int TotalKeywords, int TotalFiles, int TotalFeedback, int TotalMessages, int NegativeFeedback, int NeutralFeedback, int PositiveFeedback,
           decimal NegativeFeedbackPer, decimal NeutralFeedbackPer, decimal PositiveFeedbackPer, int FilesNotShown, int FilesShown, decimal FilesNotShownPer,
           decimal FilesShownPer)
        {
            this.TotalKeywords = TotalKeywords;
            this.TotalFiles = TotalFiles;
            this.TotalFeedback = TotalFeedback;
            this.TotalMessages = TotalMessages;
            this.NegativeFeedback = NegativeFeedback;
            this.NeutralFeedback = NeutralFeedback;
            this.PositiveFeedback = PositiveFeedback;
            this.NegativeFeedbackPer = NegativeFeedbackPer;
            this.NeutralFeedbackPer = NeutralFeedbackPer;
            this.PositiveFeedbackPer = PositiveFeedbackPer;
            this.FilesNotShown = FilesNotShown;
            this.FilesShown = FilesShown;
            this.FilesNotShownPer = FilesNotShownPer;
            this.FilesShownPer = FilesShownPer;
        }
    }
}
