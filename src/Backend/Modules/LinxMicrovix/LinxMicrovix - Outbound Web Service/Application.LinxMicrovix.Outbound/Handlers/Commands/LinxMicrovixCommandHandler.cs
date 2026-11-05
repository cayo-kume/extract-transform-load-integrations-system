using Application.LinxMicrovix.Outbound.Abstractions.Handlers.Commands;
using Domain.LinxMicrovix.Outbound.Enums;

namespace Application.LinxMicrovix.Outbound.Handlers.Commands
{
    public class LinxMicrovixCommandHandler : ILinxMicrovixCommandHandler
    {
        public string CreateGetCompanysQuery(string company)
        {
            // só os parâmetros da empresa da execução (doc_company = empresa, docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md);
            // company é só dígitos (ICompanyContext)
            // credencial e endereço do ambiente marcado na aba Parâmetros do WebApp (use_homolog_url = 1, homologação): os
            // services leem sempre as colunas de produção
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     GRUPO AS GRUPO,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HML_ERP_CHAVE ELSE ERP_CHAVE END AS ERP_CHAVE,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HML_ERP_USER_NAME ELSE ERP_USER_NAME END AS ERP_USER_NAME,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HML_ERP_PASSWORD ELSE ERP_PASSWORD END AS ERP_PASSWORD,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HML_B2C_CHAVE ELSE B2C_CHAVE END AS B2C_CHAVE,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HML_B2C_USER_NAME ELSE B2C_USER_NAME END AS B2C_USER_NAME,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HML_B2C_PASSWORD ELSE B2C_PASSWORD END AS B2C_PASSWORD,
                     CASE WHEN USE_HOMOLOG_URL = 1 THEN HOMOLOG_URL ELSE BASE_URL END AS BASE_URL
                     FROM LINXMICROVIX.LINXMICROVIXPARAMETERS P
                     WHERE P.DOC_COMPANY = '{company}'";
        }

        public string CreateGetStoresQuery(string doc_company, EnumApi api)
        {
            // lojas consultadas pelos outros métodos (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 4.2, item 3).
            // Loja sem CNPJ fica de fora: cnpjEmp é obrigatório nas chamadas. Os nomes das colunas são os da resposta da API
            // (CNPJ no LinxGrupoLojas, cnpjEmp no B2CConsultaEmpresas). doc_company vem de LinxMicrovixParameters; com aspas
            // porque a coluna é nvarchar (CNPJ pode começar com zero)
            return api switch
            {
                EnumApi.Erp => @$"SELECT DISTINCT CNPJ
                     FROM LINXMICROVIX.LINXGRUPOLOJAS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND ISNULL(CNPJ, '') <> ''
                     ORDER BY CNPJ",

                EnumApi.B2C => @$"SELECT DISTINCT CNPJEMP
                     FROM LINXMICROVIX.B2CCONSULTAEMPRESAS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND ISNULL(CNPJEMP, '') <> ''
                     ORDER BY CNPJEMP",

                _ => throw new ArgumentOutOfRangeException(nameof(api), api, "API do Microvix desconhecida.")
            };
        }

        public string CreateGetLastTimestampQuery(string doc_company, string tableName, string? cnpj_emp)
        {
            // timestamp do Microvix é o rowversion da Linx, próprio de cada método: o MAX gravado é o ponto de partida da
            // próxima leitura, e 0 (banco vazio) traz o histórico inteiro (4.2, item 4). Sem cnpj_emp: método de portal.
            // O nome da tabela vem do EnumJob (lista fechada), nunca de texto livre
            var cnpjFilter = cnpj_emp is null ? String.Empty : $"AND CNPJ_EMP = '{cnpj_emp}'";

            return @$"SELECT ISNULL(MAX([TIMESTAMP]), 0)
                     FROM LINXMICROVIX.[{tableName}]
                     WHERE DOC_COMPANY = '{doc_company}'
                     {cnpjFilter}";
        }

        public string CreateGetLastTimestampQuery(string doc_company, string tableName, string? cnpj_emp, string filterColumn, string filterValue)
        {
            // o valor do parâmetro exigido pelo método (ex.: id_tabela) vem do banco; entra entre aspas, com as aspas internas dobradas
            var cnpjFilter = cnpj_emp is null ? String.Empty : $"AND CNPJ_EMP = '{cnpj_emp}'";

            return @$"SELECT ISNULL(MAX([TIMESTAMP]), 0)
                     FROM LINXMICROVIX.[{tableName}]
                     WHERE DOC_COMPANY = '{doc_company}'
                     {cnpjFilter}
                     AND CONVERT(NVARCHAR(400), [{filterColumn}]) = '{filterValue.Replace("'", "''")}'";
        }

        public string CreateGetSourceValuesQuery(string doc_company, string sourceTable, string sourceColumn, string? cnpj_emp, string? changedSinceTable)
        {
            // valores que o método exige um por chamada, lidos de outro método já gravado. Com changedSinceTable, só os gravados
            // depois da última gravação do método que os consome: leitura incremental (ex.: cod_pedido do LinxPlanosPedidoVenda)
            var cnpjFilter = cnpj_emp is null ? String.Empty : $"AND S.CNPJ_EMP = '{cnpj_emp}'";
            var changedFilter = changedSinceTable is null
                ? String.Empty
                : $@"AND S.INTEGRATED_OR_UPDATED_AT >= ISNULL((SELECT MAX(T.INTEGRATED_OR_UPDATED_AT) FROM LINXMICROVIX.[{changedSinceTable}] T WHERE T.DOC_COMPANY = S.DOC_COMPANY {(cnpj_emp is null ? String.Empty : $"AND T.CNPJ_EMP = '{cnpj_emp}'")}), '1900-01-01')";

            return @$"SELECT DISTINCT CONVERT(NVARCHAR(400), S.[{sourceColumn}])
                     FROM LINXMICROVIX.[{sourceTable}] S
                     WHERE S.DOC_COMPANY = '{doc_company}'
                     {cnpjFilter}
                     {changedFilter}
                     AND S.[{sourceColumn}] IS NOT NULL";
        }
    }
}
