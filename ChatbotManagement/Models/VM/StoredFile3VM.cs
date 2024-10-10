namespace ChatbotManagement.Models.VM
{
    public class StoredFile3VM
    {
        public string SolrId { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string Link { get; set; }

        public bool Show { get; set; }

        public StoredFile3VM(string SolrId, string FileName, string FileType, string Link, bool Show)
        {
            this.SolrId = SolrId;
            this.FileName = FileName;
            this.FileType = FileType;
            this.Link = Link;
            this.Show = Show;
        }
    }
}
