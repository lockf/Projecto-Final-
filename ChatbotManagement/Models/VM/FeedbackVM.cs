namespace ChatbotManagement.Models.VM
{
    public class FeedbackVM
    {
        public string Opinion { get; set; }

        public string Conclusion { get; set; }
        
        public int FileId { get; set; }
        
        public string FileName { get; set; }

        public FeedbackVM(string Opinion, string Conclusion, int FileId, string FileName)
        {
            this.Opinion = Opinion;
            this.Conclusion = Conclusion;
            this.FileId = FileId;
            this.FileName = FileName;
        }
    }
}
