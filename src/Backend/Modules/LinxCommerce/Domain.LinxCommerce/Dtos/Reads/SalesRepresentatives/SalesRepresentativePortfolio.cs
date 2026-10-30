namespace Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives
{
    /// <summary>
    /// SalesRepresentativePortfolio do JSON do Linx Commerce: campos como texto, convertidos na entidade. Nomes e tipos tirados do código antigo, a conferir
    /// com uma resposta real (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public class SalesRepresentativePortfolio
    {
        public string? HasPortfolio { get; init; }
        public string? PortfolioAssociationType { get; init; }

        public List<SalesRepresentativeCustomer>? Customers { get; init; }
    }
}
