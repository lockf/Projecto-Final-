using ChatbotManagement.Models.VM;

namespace ChatbotManagement.Models.out_VM
{
    public class SecondaryInfoVM
    {
        public List<Message2VM> Message2VMs { get; set; }

        public List<string> Keywords { get; set; }

        public SecondaryInfoVM(List<Message2VM> Message2VMs, List<string> Keywords)
        {
            this.Message2VMs = Message2VMs;
            this.Keywords = Keywords;
        }
    }
}
