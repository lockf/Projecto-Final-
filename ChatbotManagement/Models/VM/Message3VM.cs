namespace ChatbotManagement.Models.VM
{
    public class Message3VM
    {
        public string Text { get; set; }

        public string ChannelId { get; set; }

        public string ConversationId { get; set; }

        public Message3VM(string Text, string ChannelId, string ConversationId)
        {
            this.Text = Text;
            this.ChannelId = ChannelId;
            this.ConversationId = ConversationId;
        }
    }
}
