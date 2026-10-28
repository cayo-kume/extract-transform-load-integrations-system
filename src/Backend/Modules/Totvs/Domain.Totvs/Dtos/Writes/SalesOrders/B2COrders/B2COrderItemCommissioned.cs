namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Comissionado. (OrderItemCommissionedInDto)
    /// </summary>
    public class B2COrderItemCommissioned
    {
        // Código do representante.
        public int? representativeCode { get; init; }
        // CPF ou CNPJ do representante.
        public string? representativeCpfCnpj { get; init; }
        public CommissionedType? commissionedType { get; init; }
        // Percentual de comissão de faturamento.
        public decimal? commissionPercentageBilling { get; init; }
        // Percentual de comissão no recebimento.
        public decimal? commissionPercentageReceipt { get; init; }
    }
}
