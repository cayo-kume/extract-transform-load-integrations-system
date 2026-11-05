using Application.LinxCommerce.Abstractions.Handlers.Commands;

namespace Application.LinxCommerce.Handlers.Commands
{
    /// <summary>
    /// queries do módulo. O doc_company vem de LinxCommerceParameters e é conferido por ^\d{14}$ no LinxCommerceService antes de
    /// chegar aqui; os ids chegam como Guid e int, convertidos pelo código (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 4.5)
    /// </summary>
    public class LinxCommerceCommandHandler : ILinxCommerceCommandHandler
    {
        public string CreateGetCompanysQuery(string company)
        {
            // só os parâmetros da empresa da execução (doc_company = empresa, docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md);
            // company é só dígitos (ICompanyContext)
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     BASE_URL AS BASE_URL,
                     USER_NAME AS USER_NAME,
                     API_KEY AS API_KEY
                     FROM LINXCOMMERCE.LINXCOMMERCEPARAMETERS P
                     WHERE P.DOC_COMPANY = '{company}'";
        }
    }
}
