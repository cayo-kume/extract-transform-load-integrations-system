using Application.LinxMicrovix.Outbound.Abstractions.Handlers.Commands;
using Domain.LinxMicrovix.Outbound.Enums;

namespace Application.LinxMicrovix.Outbound.Handlers.Commands
{
    public class LinxMicrovixCommandHandler : ILinxMicrovixCommandHandler
    {
        public string CreateGetCompanysQuery()
        {
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     GRUPO AS GRUPO,
                     ERP_CHAVE AS ERP_CHAVE,
                     ERP_USER_NAME AS ERP_USER_NAME,
                     ERP_PASSWORD AS ERP_PASSWORD,
                     B2C_CHAVE AS B2C_CHAVE,
                     B2C_USER_NAME AS B2C_USER_NAME,
                     B2C_PASSWORD AS B2C_PASSWORD
                     FROM LINXMICROVIX.LINXMICROVIXPARAMETERS";
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
