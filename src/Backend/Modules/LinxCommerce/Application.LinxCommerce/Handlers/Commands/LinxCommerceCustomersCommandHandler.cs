using Application.LinxCommerce.Abstractions.Handlers.Commands;

namespace Application.LinxCommerce.Handlers.Commands
{
    /// <summary>
    /// queries do módulo. O doc_company vem de LinxCommerceParameters e é conferido por ^\d{14}$ no LinxCommerceService antes de
    /// chegar aqui; os ids chegam como Guid e int, convertidos pelo código (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 4.5)
    /// </summary>
    public class LinxCommerceCustomersCommandHandler : ILinxCommerceCustomersCommandHandler
    {
        public string CreateGetCustomersLastBatchTimeQuery(string doc_company)
        {
            // o cliente não tem data de alteração no código antigo: a janela sai do último lote gravado (4.2, item 3)
            return @$"SELECT MAX(INTEGRATED_OR_UPDATED_AT)
                     FROM LINXCOMMERCE.LINXCOMMERCECUSTOMERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }
    }
}
