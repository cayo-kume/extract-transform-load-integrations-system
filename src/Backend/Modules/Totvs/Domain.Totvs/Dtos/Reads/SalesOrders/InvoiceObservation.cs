namespace Domain.Totvs.Dtos.Reads.SalesOrders
{
    /// <summary>
    /// observação da nota do pedido (sem chave na API: a posição é a chave)
    /// </summary>
    public class InvoiceObservation
    {
        public string? observation { get; init; }
    }
}
