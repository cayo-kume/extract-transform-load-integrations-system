namespace Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// Tipo financeiro. (PaymentType): vai como número (T3)
    /// </summary>
    public enum B2CPaymentType
    {
        Normal = 0,   // Normal
        SplitOmni = 1,   // SplitOmni
        AwaitingPayment = 2,   // Aguardando Pagamento
        TotalSplit = 3   // Split Total
    }
}
