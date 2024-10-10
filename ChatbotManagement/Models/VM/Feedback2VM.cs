namespace ChatbotManagement.Models.VM
{
    public class Feedback2VM
    {
        public string Opinion { get; set; }
        
        public string FileId { get; set; }

        public Feedback2VM(string Opinion, string FileId)
        {
            this.Opinion = Opinion;
            this.FileId = FileId;
        }
    }
}
