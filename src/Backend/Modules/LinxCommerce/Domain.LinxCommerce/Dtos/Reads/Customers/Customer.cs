namespace Domain.LinxCommerce.Dtos.Reads.Customers
{
    /// <summary>
    /// Customer do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class Customer
    {
        public string? CustomerID { get; init; }
        public string? Surname { get; init; }
        public string? BirthDate { get; init; }
        public string? Gender { get; init; }
        public string? RG { get; init; }
        public string? Cpf { get; init; }
        public string? CreatedDate { get; init; }
        public string? CustomerStatusID { get; init; }
        public string? WebSiteID { get; init; }
        public string? Name { get; init; }
        public string? Email { get; init; }
        public string? CustomerType { get; init; }
        public string? Cnpj { get; init; }
        public string? TradingName { get; init; }

        public CustomerContact? Contact { get; init; }
        public CustomerEmailConfirmation? EmailConfirmation { get; init; }
        public List<CustomerAddress>? Address { get; init; }
        public List<CustomerGroup>? Groups { get; init; }
    }
}
