namespace Domain.Totvs.Dtos.Fiscal
{
    public class ProductItem
    {
        public string sequence { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string ncm { get; set; }
        public string cfop { get; set; }
        public string measureUnit { get; set; }
        public string quantity { get; set; }
        public string grossValue { get; set; }
        public string discountValue { get; set; }
        public string netValue { get; set; }
        public string unitGrossValue { get; set; }
        public string unitDiscountValue { get; set; }
        public string unitNetValue { get; set; }
        public string additionalValue { get; set; }
        public string freightValue { get; set; }
        public string insuranceValue { get; set; }
        public string cest { get; set; }
        public string additionalItemInformation { get; set; }
        public List<Product> products { get; set; }
        public List<Tax> taxes { get; set; }
        public List<Barcode> barCodes { get; set; }
        public RateDifferential rateDifferential { get; set; }
    }
}
