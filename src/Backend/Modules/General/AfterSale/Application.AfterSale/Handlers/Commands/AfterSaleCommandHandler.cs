using Application.AfterSale.Abstractions.Handlers.Commands;

namespace Application.AfterSale.Handlers.Commands
{
    public class AfterSaleCommandHandler : IAfterSaleCommandHandler
    {
        public string CreateGetCompanyQuery(string doc_company)
        {
            // credencial e endereço do ambiente marcado na aba Parâmetros do WebApp (use_homolog_url = 1, homologação): os
            // services leem sempre as colunas de produção
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HML_TOKEN ELSE TOKEN END AS TOKEN,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HOMOLOG_URL ELSE BASE_URL END AS BASE_URL
                     FROM AFTERSALE.AFTERSALEPARAMETERS
                     WHERE 
                     DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCompanysQuery(string company)
        {
            // só os parâmetros da empresa da execução (doc_company = empresa, .docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md);
            // company é só dígitos (ICompanyContext)
            // credencial e endereço do ambiente marcado na aba Parâmetros do WebApp (use_homolog_url = 1, homologação): os
            // services leem sempre as colunas de produção
            return @$"SELECT DISTINCT
                    DOC_COMPANY AS DOC_COMPANY,
                    CASE WHEN USE_HOMOLOG_URL = 1 THEN HML_TOKEN ELSE TOKEN END AS TOKEN,
                    CASE WHEN USE_HOMOLOG_URL = 1 THEN HOMOLOG_URL ELSE BASE_URL END AS BASE_URL
                    FROM AFTERSALE.AFTERSALEPARAMETERS P
                     WHERE P.DOC_COMPANY = '{company}'";
        }
    }
}
