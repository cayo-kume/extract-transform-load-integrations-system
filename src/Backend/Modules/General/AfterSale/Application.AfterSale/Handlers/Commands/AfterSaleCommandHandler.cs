using Application.AfterSale.Abstractions.Handlers.Commands;

namespace Application.AfterSale.Handlers.Commands
{
    public class AfterSaleCommandHandler : IAfterSaleCommandHandler
    {
        public string CreateGetCompanyQuery(string cnpj_emp)
        {
            return @$"SELECT DISTINCT
                     CNPJ_EMP AS DOC_COMPANY,
                     TOKEN
                     FROM GENERAL.AFTERSALEPARAMETERS
                     WHERE 
                     CNPJ_EMP = {cnpj_emp}";
        }

        public string CreateGetCompanysQuery()
        {
            return @$"SELECT DISTINCT
                    CNPJ_EMP AS DOC_COMPANY,
                    TOKEN AS TOKEN
                    FROM GENERAL.AFTERSALEPARAMETERS
                    --WHERE CNPJ_EMP = '38367316000199'";
        }

        public string CreateGetReversesByIdsQuery(List<string> reversesIds)
        {
            string identificadores = String.Empty;

            for (int j = 0; j < reversesIds.Count(); j++)
            {
                if (j == reversesIds.Count() - 1)
                    identificadores += $"'{reversesIds[j]}'";
                else
                    identificadores += $"'{reversesIds[j]}', ";
            }

            return $"SELECT ID, UPDATED_AT FROM [GENERAL].[AFTERSALEREVERSES] WHERE ID IN ({identificadores})";
        }

        public string CreateGetSummarizedReversesIdsForGetReverseQuery()
        {
            return @$"SELECT TOP 30
                        A.CNPJ_EMP,
                        A.ID
                      FROM 
                        [GENERAL].[AFTERSALESUMMARIZEDREVERSES] A (NOLOCK)
                      LEFT JOIN
                        [GENERAL].[AFTERSALEREVERSES] B (NOLOCK) ON A.ID = B.ID
                      WHERE
                        B.ID IS NULL";
        }

        public string CreateGetSummarizedReversesIdsForGetCourierAttributesQuery()
        {
            return @$"SELECT 
                        ID
                      FROM 
                        [GENERAL].[AFTERSALESUMMARIZEDREVERSES] A (NOLOCK)
                      LEFT JOIN
                        [GENERAL].[AFTERSALECOURIERATTRIBUTES] B (NOLOCK) ON A.ID = B.ID
                      WEHRE
                        B.ID IS NULL";
        }

        public string CreateGetSummarizedReversesByIdsQuery(List<string> reversesIds)
        {
            string identificadores = String.Empty;

            for (int j = 0; j < reversesIds.Count(); j++)
            {
                if (j == reversesIds.Count() - 1)
                    identificadores += $"'{reversesIds[j]}'";
                else
                    identificadores += $"'{reversesIds[j]}', ";
            }

            return $"SELECT ID, UPDATED_AT FROM [GENERAL].[AFTERSALESUMMARIZEDREVERSES] WHERE ID IN ({identificadores})";
        }

        public string CreateGetStatusQuery(/*string doc_company*/)
        {
            return @$"SELECT * FROM [GENERAL].[AFTERSALESTATUSREVERSES] A (NOLOCK)";
            //return @$"SELECT * FROM [GENERAL].[AFTERSALESTATUSREVERSES] A (NOLOCK) --WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetTransportationsQuery(/*string doc_company*/)
        {
            return @$"SELECT TRANSPORTATION FROM [GENERAL].[AFTERSALETRANSPORTATIONSREVERSES] A (NOLOCK)";
            //return @$"SELECT TRANSPORTATION FROM [GENERAL].[AFTERSALETRANSPORTATIONSREVERSES] A (NOLOCK) --WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCourierAttributesByReverseIdQuery(List<Int64> reversesIds)
        {
            string identificadores = String.Empty;

            for (int j = 0; j < reversesIds.Count(); j++)
            {
                if (j == reversesIds.Count() - 1)
                    identificadores += $"'{reversesIds[j]}'";
                else
                    identificadores += $"'{reversesIds[j]}', ";
            }

            return $"SELECT ID, UPDATED_AT FROM [GENERAL].[AFTERSALECOURIERATTRIBUTES] WHERE ID IN ({identificadores})";

        }
    }
}
