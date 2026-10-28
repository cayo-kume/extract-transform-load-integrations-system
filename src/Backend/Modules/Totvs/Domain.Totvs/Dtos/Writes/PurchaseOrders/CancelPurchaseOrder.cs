namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// corpo da rota de cancelamento do pedido de compra: loja e pedido (o CancelOrderCommand da Totvs) e o fornecedor esperado (DP3), conferido
    /// com o pedido gravado antes de chamar e nunca enviado à Totvs. Irreversível pela API
    /// </summary>
    public class CancelPurchaseOrder
    {
        public int? branchCode { get; init; }
        public int? orderCode { get; init; }
        // fornecedor que o chamador espera no pedido: um ou outro
        public int? supplierCode { get; init; }
        public string? supplierCpfCnpj { get; init; }
    }
}
