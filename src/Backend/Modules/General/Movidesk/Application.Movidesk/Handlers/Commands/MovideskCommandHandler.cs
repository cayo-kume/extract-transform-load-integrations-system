using Application.Movidesk.Abstractions.Handlers.Commands;

namespace Application.Movidesk.Handlers.Commands
{
    public class MovideskCommandHandler : IMovideskCommandHandler
    {
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
                     FROM MOVIDESK.MOVIDESKPARAMETERS P
                     WHERE P.DOC_COMPANY = '{company}'";
        }

        public string CreateGetCompanyQuery(string doc_company)
        {
            // doc_company já passou pelo ^\d{14}$ do controller; com aspas porque a coluna é nvarchar (CNPJ pode começar com zero)
            // credencial e endereço do ambiente marcado na aba Parâmetros do WebApp (use_homolog_url = 1, homologação): os
            // services leem sempre as colunas de produção
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HML_TOKEN ELSE TOKEN END AS TOKEN,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HOMOLOG_URL ELSE BASE_URL END AS BASE_URL
                     FROM MOVIDESK.MOVIDESKPARAMETERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }
    }
}
