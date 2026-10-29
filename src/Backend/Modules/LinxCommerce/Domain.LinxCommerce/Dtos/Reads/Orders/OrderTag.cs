namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// OrderTag do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class OrderTag
    {
        public string? TagID { get; init; }
        public string? Alias { get; init; }
        public string? Name { get; init; }
        public string? IsSystem { get; init; }
        public string? IsDeleted { get; init; }
    }
}
