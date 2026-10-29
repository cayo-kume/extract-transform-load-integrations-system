namespace Domain.LinxCommerce.Dtos.Reads.Orders
{
    /// <summary>
    /// SearchOrdersResult do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SearchOrdersResult
    {
        public string? OrderID { get; init; }
        public string? OrderNumber { get; init; }
        public string? ModifiedDate { get; init; }
    }
}
