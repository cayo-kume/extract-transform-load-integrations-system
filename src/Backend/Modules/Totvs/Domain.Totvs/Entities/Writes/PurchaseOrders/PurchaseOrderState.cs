namespace Domain.Totvs.Entities.Writes.PurchaseOrders
{
    /// <summary>
    /// situação e fornecedor de um pedido de compra gravado: conferência local do cancelamento (DP3, DP7)
    /// </summary>
    public class PurchaseOrderState
    {
        public string? status { get; set; }
        public int? supplierCode { get; set; }
        public string? supplierCpfCnpj { get; set; }
    }
}
