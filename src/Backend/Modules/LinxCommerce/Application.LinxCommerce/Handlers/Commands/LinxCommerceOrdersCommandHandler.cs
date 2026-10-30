using Application.LinxCommerce.Abstractions.Handlers.Commands;

namespace Application.LinxCommerce.Handlers.Commands
{
    /// <summary>
    /// queries do módulo. O doc_company vem de LinxCommerceParameters e é conferido por ^\d{14}$ no LinxCommerceService antes de
    /// chegar aqui; os ids chegam como Guid e int, convertidos pelo código (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 4.5)
    /// </summary>
    public class LinxCommerceOrdersCommandHandler : ILinxCommerceOrdersCommandHandler
    {
        public string CreateGetOrdersLastModifiedDateQuery(string doc_company)
        {
            return @$"SELECT MAX(MODIFIEDDATE)
                     FROM LINXCOMMERCE.LINXCOMMERCEORDERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetOrdersByIdsQuery(string doc_company, List<Guid> ids)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, ORDERID, MODIFIEDDATE
                     FROM LINXCOMMERCE.LINXCOMMERCEORDERS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND ORDERID IN ({String.Join(", ", ids.Select(x => $"'{x}'"))})";
        }
    }
}
