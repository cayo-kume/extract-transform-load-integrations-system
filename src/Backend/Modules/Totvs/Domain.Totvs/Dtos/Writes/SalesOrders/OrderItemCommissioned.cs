namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// comissionado do item (AddOrderItemsCommissionedInDto): representante pelo código ou pelo CPF/CNPJ
    /// </summary>
    public class OrderItemCommissioned
    {
        public int? representativeCode { get; init; }
        public string? representativeCpfCnpj { get; init; }
        public CommissionedType? commissionedType { get; init; }
        public decimal? commissionPercentageBilling { get; init; }
        public decimal? commissionPercentageReceipt { get; init; }
    }
}
