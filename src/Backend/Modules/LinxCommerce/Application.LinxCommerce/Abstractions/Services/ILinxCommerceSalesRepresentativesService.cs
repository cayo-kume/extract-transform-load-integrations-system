namespace Application.LinxCommerce.Abstractions.Services
{
    public interface ILinxCommerceSalesRepresentativesService
    {
        /// <summary>
        /// sincroniza os vendedores (POST Sales/SearchSalesRepresentative, lidos inteiros, e GetSalesRepresentative de cada um).
        /// Só leitura: a escrita SaveSalesRepresentative do código antigo não foi migrada (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 4.6)
        /// </summary>
        public Task<bool?> SearchSalesRepresentative();

        /// <summary>
        /// busca um vendedor pelo SalesRepresentativeID (POST Sales/GetSalesRepresentative) e grava; a chamada é registrada em
        /// linxcommerce.LinxCommerceRequests. true gravado, false não existe no Linx Commerce, null empresa não cadastrada ou
        /// resposta inválida
        /// </summary>
        public Task<bool?> GetSalesRepresentative(string user_name, string doc_company, int salesRepresentativeId);
    }
}
