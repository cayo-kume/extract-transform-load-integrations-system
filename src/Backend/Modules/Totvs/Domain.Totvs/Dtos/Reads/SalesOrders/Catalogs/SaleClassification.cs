namespace Domain.Totvs.Dtos.Reads.SalesOrders.Catalogs
{
    /// <summary>
    /// classificação de pedido de venda e o seu tipo (GET sales-order/v2/classifications, ClassificationTypeRelationshipModel; type vem como número, 1 capa e 2 item, testado)
    /// </summary>
    public class SaleClassification
    {
        public string? typeCode { get; init; }
        public string? typeDescription { get; init; }
        public string? type { get; init; }
        public string? code { get; init; }
        public string? description { get; init; }
        public string? maxChangeFilterDate { get; init; }
    }
}
