namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// nota fiscal do pedido (code e serial nulos viram 0 e vazio, fazem parte da chave)
    /// </summary>
    public class Invoice
    {
        public string? code { get; init; }
        public string? serial { get; init; }
        public string? accessKey { get; init; }
        public string? issueDate { get; init; }
        public string? status { get; init; }
        public string? shippingCompanyName { get; init; }
        public string? packageNumber { get; init; }
        public string? grossWeight { get; init; }
        public string? netWeight { get; init; }
        public string? trackingCode { get; init; }
        public string? discountPercentage { get; init; }
        public string? quantity { get; init; }
        public string? productValue { get; init; }
        public string? additionalValue { get; init; }
        public string? shippingValue { get; init; }
        public string? InsuranceValue { get; init; }
        public string? ipiValue { get; init; }
        public string? totalValue { get; init; }
        public string? transactionBranchCode { get; init; }
        public string? transactionDate { get; init; }
        public string? transactionCode { get; init; }

        public Electronic? electronic { get; init; }
    }
}
