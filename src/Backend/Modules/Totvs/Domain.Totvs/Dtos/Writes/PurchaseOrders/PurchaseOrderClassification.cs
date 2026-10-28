namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// classificação do pedido de compra (PurchaseOrderClassificationModel): os dois campos ou nenhum
    /// </summary>
    public class PurchaseOrderClassification
    {
        public int? classificationTypeCode { get; init; }
        public string? classificationCode { get; init; }
    }
}
