namespace Domain.Totvs.Dtos.Reads.Products.Catalogs
{
    /// <summary>
    /// classificação de pessoa da tabela de preço (PersonClassificationModel)
    /// </summary>
    public class PriceTablePersonClassification
    {
        public string? typeCode { get; init; }
        public string? typeName { get; init; }
        public string? code { get; init; }
        public string? name { get; init; }
    }
}
