using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsCommandHandler : ITotvsCommandHandler
    {
        public string CreateGetCompanysQuery()
        {
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     CLIENT_ID AS CLIENT_ID,
                     CLIENT_SECRET AS CLIENT_SECRET,
                     USERNAME AS USERNAME,
                     [PASSWORD] AS [PASSWORD]
                     FROM TOTVS.TOTVSPARAMETERS";
        }

        public string CreateGetBranchCodesQuery(string doc_company)
        {
            // lojas da holding usadas no branchCodeList das buscas (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 4.2)
            return @$"SELECT CODE
                     FROM TOTVS.TOTVSBRANCHES
                     WHERE DOC_COMPANY = '{doc_company}'
                     ORDER BY CODE";
        }

        public string CreateGetBrandBranchCodesQuery(string doc_company, int baseBranchCode)
        {
            // lojas da marca: mesma raiz de CNPJ (8 primeiros dígitos) da matriz usada como empresa base dos produtos
            return @$"SELECT B.CODE
                     FROM TOTVS.TOTVSBRANCHES B
                     INNER JOIN TOTVS.TOTVSBRANCHES M ON M.DOC_COMPANY = B.DOC_COMPANY AND M.CODE = {baseBranchCode}
                     WHERE B.DOC_COMPANY = '{doc_company}'
                     AND LEFT(B.CNPJ, 8) = LEFT(M.CNPJ, 8)
                     ORDER BY B.CODE";
        }

        public string CreateGetBranchCodeByCnpjQuery(string doc_company, string cnpj)
        {
            // loja da empresa pelo CNPJ (escritas: o corpo da Totvs identifica a loja pelo CNPJ). O cnpj chega validado (14 dígitos)
            return @$"SELECT CODE
                     FROM TOTVS.TOTVSBRANCHES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND CNPJ = '{cnpj}'";
        }

        public string CreateGetRecentRequestsQuery(string doc_company, string route, string field_key_value, DateTime since)
        {
            // trava de repetição das escritas (T5): o corpo é comparado no service, não na query (é texto de quem chamou). Rota e chave
            // cortadas como na gravação (400) e com as aspas dobradas; o índice IX_TotvsRequests_Company_Key atende o filtro
            static string Text(string value) => (value.Length > 400 ? value[..400] : value).Replace("'", "''");

            return @$"SELECT REQUEST_DATE, REQUEST_BODY, IS_SUCCESS, STATUS_CODE
                     FROM TOTVS.TOTVSREQUESTS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND FIELD_KEY_VALUE = N'{Text(field_key_value)}'
                     AND [ROUTE] = N'{Text(route)}'
                     AND REQUEST_DATE >= '{since:yyyy-MM-ddTHH:mm:ss}'
                     ORDER BY REQUEST_DATE DESC";
        }
    }
}
