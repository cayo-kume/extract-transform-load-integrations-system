namespace Domain.LinxCommerce.Dtos.Reads.Customers
{
    /// <summary>
    /// CustomerEmailConfirmation do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class CustomerEmailConfirmation
    {
        public string? Status { get; init; }
        public string? ConfirmationDate { get; init; }
    }
}
