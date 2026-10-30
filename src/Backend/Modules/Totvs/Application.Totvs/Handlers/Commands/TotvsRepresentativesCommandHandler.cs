using Application.Totvs.Abstractions.Handlers.Commands;

namespace Application.Totvs.Handlers.Commands
{
    public class TotvsRepresentativesCommandHandler : ITotvsRepresentativesCommandHandler
    {
        public string CreateGetRepresentativesLastChangeQuery(string doc_company)
        {
            return @$"SELECT MAX(MAXCHANGEFILTERDATE)
                     FROM TOTVS.TOTVSREPRESENTATIVES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetRepresentativesByCodesQuery(string doc_company, List<int> codes)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, CODE, MAXCHANGEFILTERDATE
                     FROM TOTVS.TOTVSREPRESENTATIVES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND CODE IN ({String.Join(", ", codes)})";
        }

        public string CreateGetReceptionSalesLastMovementDateQuery(string doc_company)
        {
            return @$"SELECT MAX(MOVEMENTDATE)
                     FROM TOTVS.TOTVSRECEPTIONSALES
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetReceptionSalesHashesQuery(string doc_company, DateTime startMovementDate, DateTime endMovementDate)
        {
            // recepções gravadas da fatia, com o hash do JSON (a resposta da API não tem data de alteração). Datas no estilo 126 (ISO)
            return @$"SELECT DOC_COMPANY, BRANCHCODE, MOVEMENTDATE, SEQUENCE, JSON_HASH
                     FROM TOTVS.TOTVSRECEPTIONSALES
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND MOVEMENTDATE >= CONVERT(DATETIME, '{startMovementDate:yyyy-MM-dd}T00:00:00', 126)
                     AND MOVEMENTDATE < CONVERT(DATETIME, '{endMovementDate.AddDays(1):yyyy-MM-dd}T00:00:00', 126)";
        }
    }
}
