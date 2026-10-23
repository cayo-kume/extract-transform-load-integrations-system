namespace Domain.Totvs.Dtos.Reads.PurchaseOrders
{
    /// <summary>
    /// pedido de compra do POST purchase-order/v2/search
    /// </summary>
    public class PurchaseOrder
    {
        public string? branchCode { get; init; }
        public string? orderCode { get; init; }
        public string? supplierOrderCode { get; init; }
        public string? supplierCode { get; init; }
        public string? supplierCpfCnpj { get; init; }
        public string? supplierName { get; init; }
        public string? buyerCode { get; init; }
        public string? buyerCpfCnpj { get; init; }
        public string? buyerName { get; init; }
        public string? operationCode { get; init; }
        public string? operationName { get; init; }
        public string? shippingCompanyName { get; init; }
        public string? paymentConditionCode { get; init; }
        public string? paymentConditionName { get; init; }
        public string? paymentType { get; init; }
        public string? status { get; init; }
        public string? freightType { get; init; }
        public string? maxChangeFilterDate { get; init; }
        public string? registrationDate { get; init; }
        public string? deliveryForecastDate { get; init; }
        public string? deliveryDeadlineDate { get; init; }
        public string? basePaymentDate { get; init; }
        public string? productValue { get; init; }
        public string? quantity { get; init; }
        public string? ipiValue { get; init; }
        public string? totalAmountOrder { get; init; }

        public List<Product>? items { get; init; }
        public List<Request>? requests { get; init; }
    }
}
