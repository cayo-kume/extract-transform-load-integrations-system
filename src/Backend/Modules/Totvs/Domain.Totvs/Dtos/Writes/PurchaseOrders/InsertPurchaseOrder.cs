namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// corpo do POST purchase-order/v2/orders (InsertPurchaseOrderCommand): inclui um pedido de compra; a resposta traz loja e número.
    /// O supplierOrderCode é obrigatório na rota (DP2): com ele o service recusa a repetição de um pedido que já está no banco
    /// </summary>
    public class InsertPurchaseOrder
    {
        public int? branchCode { get; init; }
        // fornecedor e comprador: cada um pelo código ou pelo CPF/CNPJ
        public int? supplierCode { get; init; }
        public string? supplierCpfCnpj { get; init; }
        public int? buyerCode { get; init; }
        public string? buyerCpfCnpj { get; init; }
        public int? operationCode { get; init; }
        public int? paymentConditionCode { get; init; }
        public StatusOrderCreateType? status { get; init; }
        public DateTime? orderDate { get; init; }
        public string? supplierOrderCode { get; init; }
        public PaymentType? paymentType { get; init; }
        // transportadora: no máximo um dos dois
        public int? shippingCompanyCode { get; init; }
        public string? shippingCompanyCpfCnpj { get; init; }
        public FreightType? freightType { get; init; }
        public DateTime? deliveryForecastDate { get; init; }
        public DateTime? deliveryDeadlineDate { get; init; }
        public DateTime? basePaymentDate { get; init; }
        // a Totvs confere com os itens, descontos e despesas
        public decimal? totalAmountOrder { get; init; }
        public List<PurchaseOrderItem>? items { get; init; }
        public List<PurchaseOrderClassification>? classifications { get; init; }
        public List<PurchaseOrderObservation>? observations { get; init; }
        public List<PurchaseOrderExpense>? expenses { get; init; }
        public List<PurchaseOrderDiscount>? discounts { get; init; }
        public List<PurchaseOrderAdditionalField>? additionalFields { get; init; }
        public PurchaseOrderAdditionalData? additionalData { get; init; }
    }
}
