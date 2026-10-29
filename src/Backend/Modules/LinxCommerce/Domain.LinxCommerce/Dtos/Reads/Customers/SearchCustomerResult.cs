namespace Domain.LinxCommerce.Dtos.Reads.Customers
{
    /// <summary>
    /// SearchCustomerResult do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SearchCustomerResult
    {
        public string? CustomerID { get; init; }
        public string? CustomerType { get; init; }
    }
}
