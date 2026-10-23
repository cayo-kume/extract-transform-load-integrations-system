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

        public string CreateGetCompanysQuery()
        {
            return @$"SELECT DISTINCT
                    DOC_COMPANY,
                    TOKEN AS TOKEN
                    FROM AFTERSALE.AFTERSALEPARAMETERS
                    --WHERE DOC_COMPANY = '38367316000199'";
        }

        public string CreateGetReversesByIdsQuery(string doc_company, List<string> reversesIds)
        {
            string identificadores = String.Empty;

            for (int j = 0; j < reversesIds.Count(); j++)
            {
                if (j == reversesIds.Count() - 1)
                    identificadores += $"'{reversesIds[j]}'";
                else
                    identificadores += $"'{reversesIds[j]}', ";
            }

            return $"SELECT ID, DOC_COMPANY, UPDATED_AT FROM [AFTERSALE].[AFTERSALEREVERSES] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}' AND ID IN ({identificadores})";
        }

        public string CreateGetSummarizedReversesIdsForGetReverseQuery()
        {
            return @$"SELECT TOP 30
                        A.DOC_COMPANY,
                        A.ID
                      FROM
                        [AFTERSALE].[AFTERSALESUMMARIZEDREVERSES] A (NOLOCK)
                      LEFT JOIN
                        [AFTERSALE].[AFTERSALEREVERSES] B (NOLOCK) ON A.DOC_COMPANY = B.DOC_COMPANY AND A.ID = B.ID
                      WHERE
                        B.ID IS NULL";
        }

        public string CreateGetSummarizedReversesIdsForGetCourierAttributesQuery()
        {
            return @$"SELECT TOP 30
                        A.DOC_COMPANY,
                        A.ID
                      FROM
                        [AFTERSALE].[AFTERSALESUMMARIZEDREVERSES] A (NOLOCK)
                      LEFT JOIN
                        [AFTERSALE].[AFTERSALECOURIERATTRIBUTES] B (NOLOCK) ON A.DOC_COMPANY = B.DOC_COMPANY AND A.ID = B.REVERSE_ID
                      WHERE
                        B.REVERSE_ID IS NULL";
        }

        public string CreateGetSummarizedReversesByIdsQuery(string doc_company, List<string> reversesIds)
        {
            string identificadores = String.Empty;

            for (int j = 0; j < reversesIds.Count(); j++)
            {
                if (j == reversesIds.Count() - 1)
                    identificadores += $"'{reversesIds[j]}'";
                else
                    identificadores += $"'{reversesIds[j]}', ";
            }

            return $"SELECT ID, DOC_COMPANY, UPDATED_AT FROM [AFTERSALE].[AFTERSALESUMMARIZEDREVERSES] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}' AND ID IN ({identificadores})";
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

        public string CreateGetSummarizedRefundsIdsForGetRefundQuery()
        {
            return @$"SELECT TOP 30
                        A.DOC_COMPANY,
                        A.ID
                      FROM
                        [AFTERSALE].[AFTERSALESUMMARIZEDREFUNDS] A (NOLOCK)
                      LEFT JOIN
                        [AFTERSALE].[AFTERSALEREFUNDS] B (NOLOCK) ON A.DOC_COMPANY = B.DOC_COMPANY AND A.ID = B.ID
                      WHERE
                        B.ID IS NULL";
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

        public string CreateGetStatusQuery(string doc_company)
        {
            return @$"SELECT * FROM [AFTERSALE].[AFTERSALESTATUSREVERSES] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetTransportationsQuery(string doc_company)
        {
            return @$"SELECT DOC_COMPANY, TRANSPORTATION FROM [AFTERSALE].[AFTERSALETRANSPORTATIONSREVERSES] A (NOLOCK) WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCourierAttributesByReverseIdQuery(string doc_company, List<Int64> reversesIds)
        {
            string identificadores = String.Empty;

            for (int j = 0; j < reversesIds.Count(); j++)
            {
                if (j == reversesIds.Count() - 1)
                    identificadores += $"'{reversesIds[j]}'";
                else
                    identificadores += $"'{reversesIds[j]}', ";
            }

            return $"SELECT REVERSE_ID, DOC_COMPANY, FILL_FORM_FROM_FILE FROM [AFTERSALE].[AFTERSALECOURIERATTRIBUTES] (NOLOCK) WHERE DOC_COMPANY = '{doc_company}' AND REVERSE_ID IN ({identificadores})";

        }
    }
}
