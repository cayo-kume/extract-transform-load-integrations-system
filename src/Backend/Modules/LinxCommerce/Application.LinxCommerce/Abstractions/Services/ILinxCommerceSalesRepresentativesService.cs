namespace Application.LinxCommerce.Abstractions.Services
{
    public interface ILinxCommerceSalesRepresentativesService
    {
        /// <summary>
        /// sincroniza os vendedores (POST Sales/SearchSalesRepresentative, lidos inteiros, e GetSalesRepresentative de cada um).
        /// Só leitura: a escrita SaveSalesRepresentative do código antigo não foi migrada (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 4.6)
        /// </summary>
        public Task<bool?> SearchSalesRepresentative();
    }
}
