using Application.AfterSale.Abstractions.Handlers.Commands;

namespace Application.AfterSale.Handlers.Commands
{
    public class AfterSaleCommandHandler : IAfterSaleCommandHandler
    {
        public string CreateGetCompanyQuery(string doc_company)
        {
            return @$"SELECT DISTINCT
                     DOC_COMPANY,
                     TOKEN
                     FROM AFTERSALE.AFTERSALEPARAMETERS
                     WHERE 
                     DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCompanysQuery(string company)
        {
            // só os parâmetros da empresa da execução (doc_company = empresa, docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md);
            // company é só dígitos (ICompanyContext)
            return @$"SELECT DISTINCT
                    DOC_COMPANY,
                    TOKEN AS TOKEN
                    FROM AFTERSALE.AFTERSALEPARAMETERS P
                     WHERE P.DOC_COMPANY = '{company}'";
        }
    }
}
