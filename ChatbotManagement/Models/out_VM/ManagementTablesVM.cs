using ChatbotManagement.Models.VM;

namespace ChatbotManagement.Models.out_VM
{
    public class ManagementTablesVM
    {
        public List<StoredFile2VM> StoredFile2VM { get; set; }

        public List<StoredFile3VM> StoredFile3VM { get; set; }

        public ManagementTablesVM(List<StoredFile2VM> StoredFile2VM, List<StoredFile3VM> StoredFile3VM)
        {
            this.StoredFile2VM = StoredFile2VM;
            this.StoredFile3VM = StoredFile3VM;
        }
    }
}
