using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsSellersCommandHandler : ITotvsSellersCommandHandler
    {
        public string CreateGetSellersLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSSELLERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetSellersByCodesQuery(string doc_company, List<int> codes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, SELLERCODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSSELLERS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND SELLERCODE IN ({String.Join(", ", codes)})";
        }

        public string CreateGetOperationalAreasHashesQuery(string doc_company)
        {
            // regiões gravadas da empresa, com o hash da região agrupada (a resposta da API não tem data de alteração sem filtro)
            return @$"SELECT DOC_COMPANY, OPERATIONALAREACODE, JSON_HASH
                     FROM TOTVS.TOTVSOPERATIONALAREAS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }
    }
}
