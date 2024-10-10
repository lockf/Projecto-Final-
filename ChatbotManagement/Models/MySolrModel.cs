using SolrNet.Attributes;

namespace ChatbotManagement.Models
{
    public class MySolrModel
    {
        [SolrUniqueKey("id")]
        public string SolrId { get; set; }

        [SolrField("title")]
        public string FileName { get; set; }

        [SolrField("keywords")]
        public string Keywords { get; set; }

        [SolrField("content_type")]
        public string[] FileType { get; set; }

        public MySolrModel()
        {
            this.SolrId = null!;
            this.FileName = null!;
            this.Keywords = null!;
            this.FileType = null!;
        }

        public MySolrModel(string SolrId, string FileName, string Keywords, string[] FileType) 
        {
            this.SolrId = SolrId;
            this.FileName = FileName;
            this.Keywords = Keywords;
            this.FileType = FileType;
        }
    }
}
