using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsPeopleCommandHandler : ITotvsPeopleCommandHandler
    {
        public string CreateGetBranchesLastChangeQuery(string doc_company)
        {
            // doc_company vem de TotvsParameters; com aspas porque a coluna é nvarchar (CNPJ pode começar com zero)
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSBRANCHES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetBranchesByCodesQuery(string doc_company, List<int> codes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSBRANCHES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND CODE IN ({String.Join(", ", codes)})";
        }

        public string CreateGetCustomersLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSPEOPLE
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetCustomersByCodesQuery(string doc_company, List<int> codes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSPEOPLE
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND CODE IN ({String.Join(", ", codes)})";
        }

        public string CreateGetLegalEntitiesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSLEGALENTITIES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetLegalEntitiesByCodesQuery(string doc_company, List<int> codes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSLEGALENTITIES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND CODE IN ({String.Join(", ", codes)})";
        }
    }
}
