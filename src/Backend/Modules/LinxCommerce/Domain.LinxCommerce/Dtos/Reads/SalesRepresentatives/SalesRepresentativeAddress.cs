namespace Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives
{
    /// <summary>
    /// SalesRepresentativeAddress do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SalesRepresentativeAddress
    {
        public string? IsMainAddress { get; init; }
        public string? Name { get; init; }
        public string? AddressLine { get; init; }
        public string? City { get; init; }
        public string? Neighbourhood { get; init; }
        public string? Number { get; init; }
        public string? State { get; init; }
        public string? AddressNotes { get; init; }
        public string? Landmark { get; init; }
        public string? ContactName { get; init; }
        public string? Latitude { get; init; }
        public string? Longitude { get; init; }
        public string? PostalCode { get; init; }
    }
}
