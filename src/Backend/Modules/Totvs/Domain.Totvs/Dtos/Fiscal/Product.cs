namespace Domain.Totvs.Dtos.Fiscal
{
    public class Product
    {
        public string productCode { get; set; }
        public string productName { get; set; }
        public string dealerCode { get; set; }
        public string quantity { get; set; }
        public string unitGrossValue { get; set; }
        public string unitDiscountValue { get; set; }
        public string unitNetValue { get; set; }
        public string grossValue { get; set; }
        public string discountValue { get; set; }
        public string netValue { get; set; }
        public List<ReferencedInvoice> referencedInvoices { get; set; }
    }
}
