using Application.Movidesk.Abstractions.Handlers.Commands;

namespace Application.Movidesk.Handlers.Commands
{
    public class MovideskTicketsCommandHandler : IMovideskTicketsCommandHandler
    {
        public string CreateGetTicketsLastUpdateQuery(string doc_company)
        {
            return @$"SELECT MAX(LASTUPDATE)
                     FROM MOVIDESK.MOVIDESKTICKETS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetTicketsByIdsQuery(string doc_company, List<long> ids)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, ID, LASTUPDATE
                     FROM MOVIDESK.MOVIDESKTICKETS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND ID IN ({String.Join(", ", ids)})";
        }

        public string CreateGetActionsHtmlDescriptionsQuery(string doc_company, List<long> ticketIds)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, TICKETID, ID, HTMLDESCRIPTION
                     FROM MOVIDESK.MOVIDESKTICKETSACTIONS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND TICKETID IN ({String.Join(", ", ticketIds)})
                     AND HTMLDESCRIPTION IS NOT NULL";
        }
    }
}
