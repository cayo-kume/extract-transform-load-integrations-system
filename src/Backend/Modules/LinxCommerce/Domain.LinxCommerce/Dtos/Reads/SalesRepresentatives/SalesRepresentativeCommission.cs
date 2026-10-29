namespace Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives
{
    /// <summary>
    /// SalesRepresentativeCommission do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SalesRepresentativeCommission
    {
        public string? TotalCommission { get; init; }
        public string? DeliveryCommission { get; init; }
    }
}
