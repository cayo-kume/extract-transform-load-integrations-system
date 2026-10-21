using Application.Movidesk.Abstractions.Handlers.Commands;

namespace Application.Movidesk.Handlers.Commands
{
    public class MovideskCommandHandler : IMovideskCommandHandler
    {
        public string CreateGetCompanysQuery()
        {
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     TOKEN AS TOKEN
                     FROM GENERAL.MOVIDESKPARAMETERS";
        }

        public string CreateGetCompanyQuery(string doc_company)
        {
            // doc_company já passou pelo ^\d{14}$ do controller; com aspas porque a coluna é nvarchar (CNPJ pode começar com zero)
            return @$"SELECT DISTINCT
                     DOC_COMPANY AS DOC_COMPANY,
                     TOKEN AS TOKEN
                     FROM GENERAL.MOVIDESKPARAMETERS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetTicketsLastUpdateQuery(string doc_company)
        {
            return @$"SELECT MAX(LASTUPDATE)
                     FROM GENERAL.MOVIDESKTICKETS
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetTicketsByIdsQuery(string doc_company, List<long> ids)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, ID, LASTUPDATE
                     FROM GENERAL.MOVIDESKTICKETS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND ID IN ({String.Join(", ", ids)})";
        }

        public string CreateGetActionsHtmlDescriptionsQuery(string doc_company, List<long> ticketIds)
        {
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, TICKETID, ID, HTMLDESCRIPTION
                     FROM GENERAL.MOVIDESKTICKETSACTIONS
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND TICKETID IN ({String.Join(", ", ticketIds)})
                     AND HTMLDESCRIPTION IS NOT NULL";
        }

        public string CreateGetPeopleLastChangeQuery(string doc_company)
        {
            // quem nunca foi alterado tem changedDate nulo: a última mudança é a criação
            return @$"SELECT MAX(COALESCE(CHANGEDDATE, CREATEDDATE))
                     FROM GENERAL.MOVIDESKPEOPLE
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPeopleByIdsQuery(string doc_company, List<string> ids)
        {
            // o Cód. Ref. é alfanumérico: entra entre aspas, com as aspas internas dobradas
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, ID, CREATEDDATE, CHANGEDDATE
                     FROM GENERAL.MOVIDESKPEOPLE
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND ID IN ({String.Join(", ", ids.Select(id => $"'{id.Replace("'", "''")}'"))})";
        }
    }
}
