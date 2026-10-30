using Application.LinxCommerce.Abstractions.Handlers.Commands;

namespace Application.LinxCommerce.Handlers.Commands
{
    /// <summary>
    /// queries do módulo. O doc_company vem de LinxCommerceParameters e é conferido por ^\d{14}$ no LinxCommerceService antes de
    /// chegar aqui; os ids chegam como Guid e int, convertidos pelo código (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 4.5)
    /// </summary>
    public class LinxCommerceCatalogCommandHandler : ILinxCommerceCatalogCommandHandler
    {
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
