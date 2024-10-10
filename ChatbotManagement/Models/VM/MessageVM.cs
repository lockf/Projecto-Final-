namespace ChatbotManagement.Models.VM
{
    public class MessageVM
    {
        public string Text { get; set; }

        public DateTime Date { get; set; }

        public string ChannelId { get; set; }

        public string ConversationId { get; set; }

        public MessageVM(string Text, DateTime Date, string ChannelId, string ConversationId)
        {
            this.Text = Text;
            this.Date = Date;
            this.ChannelId = ChannelId;
            this.ConversationId = ConversationId;
        }
    }
}
