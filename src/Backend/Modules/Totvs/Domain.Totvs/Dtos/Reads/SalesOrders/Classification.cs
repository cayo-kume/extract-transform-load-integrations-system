namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// classificação do pedido (code nulo vira vazio, faz parte da chave)
    /// </summary>
    public class Classification
    {
        public string? typeCode { get; init; }
        public string? code { get; init; }
        public string? typeName { get; init; }
        public string? name { get; init; }
    }
}
