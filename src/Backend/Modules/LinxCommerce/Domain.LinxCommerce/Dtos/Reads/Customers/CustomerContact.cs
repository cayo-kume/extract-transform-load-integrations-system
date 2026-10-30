namespace Domain.LinxCommerce.Dtos.Reads.Customers
{
    /// <summary>
    /// CustomerContact do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class CustomerContact
    {
        public string? Phone { get; init; }
        public string? Phone2 { get; init; }
        public string? CellPhone { get; init; }
        public string? Fax { get; init; }
    }
}
