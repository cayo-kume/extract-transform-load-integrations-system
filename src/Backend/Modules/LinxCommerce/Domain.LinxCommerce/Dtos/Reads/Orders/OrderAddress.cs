namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderAddress do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderAddress
    {
        public string? OrderAddressID { get; init; }
        public string? Name { get; init; }
        public string? AddressLine { get; init; }
        public string? City { get; init; }
        public string? Neighbourhood { get; init; }
        public string? Number { get; init; }
        public string? State { get; init; }
        public string? PostalCode { get; init; }
        public string? AddressNotes { get; init; }
        public string? Landmark { get; init; }
        public string? ContactName { get; init; }
        public string? ContactDocumentNumber { get; init; }
        public string? AddressType { get; init; }
        public string? PointOfSaleID { get; init; }
        public string? ContactPhone { get; init; }
    }
}
