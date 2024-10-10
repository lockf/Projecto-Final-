namespace ChatbotManagement.Models.VM
{
    public class Message2VM
    {
        public int Id { get; set; }

        public string ChannelId { get; set; }

        public string ConversationId { get; set; }

        public DateTime Date { get; set; }

        public Message2VM( int Id, string ChannelId, string ConversationId, DateTime Date)
        {
            this.Id = Id;
            this.ChannelId = ChannelId;
            this.ConversationId = ConversationId;
            this.Date = Date;
        }
    }
}
