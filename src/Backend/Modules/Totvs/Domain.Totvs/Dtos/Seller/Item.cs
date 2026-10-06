namespace Domain.Totvs.Dtos.Seller
{
    public class Item
    {
        public string sellerCode { get; set; }
        public string sellerName { get; set; }
        public string personCode { get; set; }
        public string personName { get; set; }
        public string maxChangeFilterDate { get; set; }
        public List<BranchInformation> branchInformations { get; set; }
        public List<Customer> customers { get; set; }
    }
}
