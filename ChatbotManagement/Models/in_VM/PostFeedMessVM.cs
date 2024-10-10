using ChatbotManagement.Models.VM;

namespace ChatbotManagement.Models.in_VM
{
    public class PostFeedMessVM
    {
        public Message3VM Message3VM { get; set; }

        public Feedback2VM Feedback2VM { get; set; }

        public PostFeedMessVM(Message3VM Message3VM, Feedback2VM Feedback2VM)
        {
            this.Message3VM = Message3VM;
            this.Feedback2VM = Feedback2VM;
        }
    }
}
