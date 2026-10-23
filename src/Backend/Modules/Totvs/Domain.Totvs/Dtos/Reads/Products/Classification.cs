namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// classificação do produto (ClassificationModel)
    /// </summary>
    public class Classification
    {
        public string? typeCode { get; init; }
        public string? typeName { get; init; }
        public string? typeNameAux { get; init; }
        public string? code { get; init; }
        public string? name { get; init; }
        public string? nameAux { get; init; }
    }
}
