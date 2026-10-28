namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// tipo do documento do pedido de compra: vai como número (T3)
    /// </summary>
    public enum PaymentType
    {
        Duplicate = 1,   // duplicata
        BankSlip = 2,   // boleto bancário
        BankDeposit = 12   // depósito bancário
    }
}
