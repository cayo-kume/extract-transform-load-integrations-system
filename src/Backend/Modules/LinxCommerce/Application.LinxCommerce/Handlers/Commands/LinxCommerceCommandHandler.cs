using Application.LinxCommerce.Abstractions.Handlers.Commands;

namespace Application.LinxCommerce.Handlers.Commands
{
    /// <summary>
    /// queries do módulo. O doc_company vem de LinxCommerceParameters e é conferido por ^\d{14}$ no LinxCommerceService antes de
    /// chegar aqui; os ids chegam como Guid e int, convertidos pelo código (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 4.5)
    /// </summary>
    public class LinxCommerceCommandHandler : ILinxCommerceCommandHandler
    {
        public string CreateGetCompanysQuery()
        {
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     BASE_URL AS BASE_URL,
                     USER_NAME AS USER_NAME,
                     API_KEY AS API_KEY
                     FROM LINXCOMMERCE.LINXCOMMERCEPARAMETERS";
        }

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

        public string CreateGetCustomersLastBatchTimeQuery(string doc_company)
        {
            // o cliente não tem data de alteração no código antigo: a janela sai do último lote gravado (4.2, item 3)
            return @$"SELECT MAX(INTEGRATED_OR_UPDATED_AT)
                     FROM LINXCOMMERCE.LINXCOMMERCECUSTOMERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetProductsLastModifiedDateQuery(string doc_company)
        {
            return @$"SELECT MAX(MODIFIEDDATE)
                     FROM LINXCOMMERCE.LINXCOMMERCEPRODUCTS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetProductsByIdsQuery(string doc_company, List<int> ids)
        {
            return @$"SELECT DOC_COMPANY, PRODUCTID, MODIFIEDDATE
                     FROM LINXCOMMERCE.LINXCOMMERCEPRODUCTS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND PRODUCTID IN ({String.Join(", ", ids)})";
        }

        public string CreateGetSkusLastModifiedDateQuery(string doc_company)
        {
            return @$"SELECT MAX(MODIFIEDDATE)
                     FROM LINXCOMMERCE.LINXCOMMERCESKUS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSkusByIdsQuery(string doc_company, List<int> ids)
        {
            return @$"SELECT DOC_COMPANY, PRODUCTID, MODIFIEDDATE
                     FROM LINXCOMMERCE.LINXCOMMERCESKUS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND PRODUCTID IN ({String.Join(", ", ids)})";
        }
    }
}
