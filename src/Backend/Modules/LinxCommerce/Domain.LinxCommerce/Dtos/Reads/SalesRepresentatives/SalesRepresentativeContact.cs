namespace Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives
{
    /// <summary>
    /// SalesRepresentativeContact do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SalesRepresentativeContact
    {
        public string? Email { get; init; }
        public string? Phone { get; init; }
        public string? CellPhone { get; init; }
    }
}
