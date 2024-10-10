namespace ChatbotManagement.Models.out_VM
{
    public class StoredFileVM_out
    {
        public int Id { get; set; }

        public string Link { get; set; }

        public StoredFileVM_out(int Id, string Link) 
        {
            this.Id = Id;
            this.Link = Link;
        }
    }
}
