namespace Application.Movidesk.Abstractions.Handlers.Commands
{
    public interface IMovideskTicketsCommandHandler
    {
        public string CreateGetTicketsLastUpdateQuery(string doc_company);
        public string CreateGetTicketsByIdsQuery(string doc_company, List<long> ids);
        public string CreateGetActionsHtmlDescriptionsQuery(string doc_company, List<long> ticketIds);
    }
}
