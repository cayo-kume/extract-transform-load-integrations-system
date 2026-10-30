using Domain.Shared.Entities.Base;

namespace Domain.LinxCommerce.Entities.Parameters
{
    /// <summary>
    /// linha de linxcommerce.LinxCommerceParameters: o cliente, o endereço do ambiente dele no Linx Commerce e as credenciais
    /// do Basic (usuário:chave). Uma linha por cliente; nada fixo da Newbloomers (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.1)
    /// </summary>
    public class Company : CompanyBase
    {
        public string base_url { get; init; } = String.Empty;
        public string user_name { get; init; } = String.Empty;
        public string api_key { get; init; } = String.Empty;
    }
}
