namespace Domain.Totvs.Dtos.Reads.Products.Values
{
    /// <summary>
    /// saldos de um produto (POST product/v2/balances/search, ProductBalanceModel); measurementConversion não é pedida (é a conversão de unidade do produto, repetida em cada saldo)
    /// </summary>
    public class ProductBalance
    {
        public string? productCode { get; init; }
        public string? productName { get; init; }
        public string? productSku { get; init; }
        public string? referenceCode { get; init; }
        public string? colorCode { get; init; }
        public string? colorName { get; init; }
        public string? sizeName { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public List<ProductBalanceItem>? balances { get; init; }
        public List<ProductLocation>? locations { get; init; }
    }
}
