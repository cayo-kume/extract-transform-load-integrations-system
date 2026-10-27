namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// classificação de pedido de venda da tabela de preço (SalesOrderClassificationModel)
    /// </summary>
    public class PriceTableSalesOrderClassification
    {
        public string? typeCode { get; init; }
        public string? typeName { get; init; }
        public string? code { get; init; }
        public string? name { get; init; }
    }
}
