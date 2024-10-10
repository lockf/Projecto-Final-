using System.Security.Policy;

namespace ChatbotManagement.Models.VM
{
    public class StoredFile2VM
    {
        public string SolrId { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string Link { get; set; }

        public StoredFile2VM(string SolrId, string FileName, string FileType, string Link)
        {
            this.SolrId = SolrId;
            this.FileName = FileName;
            this.FileType = FileType;
            this.Link = Link;
        }
    }
}
