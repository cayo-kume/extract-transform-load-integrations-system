namespace Domain.Totvs.Dtos.Writes.SalesOrders
{
    /// <summary>
    /// tipo de frete (FreitghtType no swagger), com os números da descrição: vai como número (D6/T3)
    /// </summary>
    public enum FreightType
    {
        CIF = 1,                        // contratado por conta do remetente
        FOB = 2,                        // contratado por conta do destinatário
        HiredByThirdParty = 3,          // contratado por conta de terceiros
        NoTransportOccurence = 4,       // sem ocorrência de transporte
        SenderResponsability = 5,       // próprio por conta do remetente
        ReceiverResponsability = 6      // próprio por conta do destinatário
    }
}
