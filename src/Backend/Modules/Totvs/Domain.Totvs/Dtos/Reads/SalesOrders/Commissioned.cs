namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// comissionado do pedido
    /// </summary>
    public class Commissioned
    {
        public string? representativeCode { get; init; }
        public string? representativeName { get; init; }
        public string? commissionPercentageBilling { get; init; }
        public string? commissionPercentageReceipt { get; init; }
        public string? commissionedType { get; init; }
    }
}
