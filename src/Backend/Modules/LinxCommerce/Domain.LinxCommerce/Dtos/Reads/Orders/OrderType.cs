namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderType do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderType
    {
        public string? OrderTypeID { get; init; }
        public string? AllowMultiPayment { get; init; }
        public string? EmitFiscalTicket { get; init; }
        public string? IntegrationID { get; init; }
        public string? Name { get; init; }
        public string? RequireInventory { get; init; }
        public string? RequirePayment { get; init; }
    }
}
