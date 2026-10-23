namespace Domain.Totvs.Dtos.Reads.Products
{
    /// <summary>
    /// sequência de código de referência do produto (ReferenceCodeSequenceModel); code se repete
    /// </summary>
    public class ReferenceCodeSequence
    {
        public string? code { get; init; }
        public string? name { get; init; }
        public string? referenceId { get; init; }
        public string? parentCode { get; init; }
    }
}
