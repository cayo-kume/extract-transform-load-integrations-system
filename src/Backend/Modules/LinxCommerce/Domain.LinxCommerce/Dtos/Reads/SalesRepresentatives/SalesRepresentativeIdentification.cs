namespace Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives
{
    /// <summary>
    /// SalesRepresentativeIdentification do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SalesRepresentativeIdentification
    {
        public string? Type { get; init; }
        public string? DocumentNumber { get; init; }
    }
}
