namespace ChatbotManagement.Models.VM
{
    public class StoredFileVM
    {
        public string SolrId { get; set; }
        public string Keyword { get; set; }

        public StoredFileVM(string SolrId, string Keyword)
        {
            this.SolrId = SolrId;
            this.Keyword = Keyword;
        }
    }
}
