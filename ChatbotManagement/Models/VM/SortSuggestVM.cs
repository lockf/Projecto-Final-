namespace ChatbotManagement.Models.VM
{
    public class SortSuggestVM
    {
        public int Id { get; set; }

        public float Accuracy { get; set; }

        public string Link { get; set; }

        public SortSuggestVM(int Id, float Accuracy, string link)
        {
            this.Id = Id;
            this.Accuracy = Accuracy;
            this.Link = link;
        }
    }
}
