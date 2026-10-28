namespace Domain.Totvs.Dtos.Writes.PurchaseOrders
{
    /// <summary>
    /// tipo de frete do pedido de compra: vai como número (T3)
    /// </summary>
    public enum FreightType
    {
        CIF = 1,   // por conta do remetente
        FOB = 2,   // por conta do destinatário
        HiredByThirdParty = 3,   // por conta de terceiros
        NoTransportOccurence = 4,   // sem ocorrência de transporte
        SenderResponsability = 5,   // próprio por conta do remetente
        ReceiverResponsability = 6   // próprio por conta do destinatário
    }
}
