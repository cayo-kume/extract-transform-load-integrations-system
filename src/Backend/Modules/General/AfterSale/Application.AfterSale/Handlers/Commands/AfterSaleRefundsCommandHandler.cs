using Application.AfterSale.Abstractions.Handlers.Commands;

namespace Application.AfterSale.Handlers.Commands
{
    public class AfterSaleRefundsCommandHandler : IAfterSaleRefundsCommandHandler
    {
        public string CreateGetSummarizedRefundsLastUpdateQuery(string doc_company)
        {
            return $"SELECT MAX(UPDATED_AT) FROM [AFTERSALE].[AFTERSALESUMMARIZEDREFUNDS] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSummarizedRefundsByIdsQuery(string doc_company, List<string> refundsIds)
        {
            string identificadores = String.Empty;

            for (int j = 0; j < refundsIds.Count(); j++)
            {
                if (j == refundsIds.Count() - 1)
                    identificadores += $"'{refundsIds[j]}'";
                else
                    identificadores += $"'{refundsIds[j]}', ";
            }

            return $"SELECT ID, DOC_COMPANY, UPDATED_AT FROM [AFTERSALE].[AFTERSALESUMMARIZEDREFUNDS] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}' AND ID IN ({identificadores})";
        }

        public string CreateGetSummarizedRefundsIdsForGetRefundQuery(string doc_company)
        {
            // 30 por execução, da empresa da execução, em ordem de id: o limite de consultas da Send4 (cerca de 30) é respeitado pelo
            // intervalo da agenda entre uma execução e outra
            return @$"SELECT TOP 30
                        A.DOC_COMPANY,
                        A.ID
                      FROM
                        [AFTERSALE].[AFTERSALESUMMARIZEDREFUNDS] A (NOLOCK)
                      LEFT JOIN
                        [AFTERSALE].[AFTERSALEREFUNDS] B (NOLOCK) ON A.DOC_COMPANY = B.DOC_COMPANY AND A.ID = B.ID
                      WHERE
                        A.DOC_COMPANY = '{doc_company}'
                        AND B.ID IS NULL
                      ORDER BY
                        A.ID";
        }

        public string CreateGetRefundsByIdsQuery(string doc_company, List<string> refundsIds)
        {
            string identificadores = String.Empty;

            for (int j = 0; j < refundsIds.Count(); j++)
            {
                if (j == refundsIds.Count() - 1)
                    identificadores += $"'{refundsIds[j]}'";
                else
                    identificadores += $"'{refundsIds[j]}', ";
            }

            return $"SELECT ID, DOC_COMPANY, UPDATED_AT FROM [AFTERSALE].[AFTERSALEREFUNDS] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}' AND ID IN ({identificadores})";
        }

        public string CreateGetStatusRefundsQuery(string doc_company)
        {
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALESTATUSREFUNDS] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetActionsRefundsQuery(string doc_company)
        {
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALEACTIONSREFUNDS] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetBanksRefundsQuery(string doc_company)
        {
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALEBANKSREFUNDS] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetTypesRefundsQuery(string doc_company)
        {
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALETYPESREFUNDS] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }
    }
}
