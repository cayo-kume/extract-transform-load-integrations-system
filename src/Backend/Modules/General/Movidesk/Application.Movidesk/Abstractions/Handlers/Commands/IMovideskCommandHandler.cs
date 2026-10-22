namespace Application.Movidesk.Abstractions.Handlers.Commands
{
    public interface IMovideskCommandHandler
    {
        public string CreateGetCompanysQuery();
        public string CreateGetCompanyQuery(string doc_company);

        public string CreateGetTicketsLastUpdateQuery(string doc_company);
        public string CreateGetTicketsByIdsQuery(string doc_company, List<long> ids);
        public string CreateGetActionsHtmlDescriptionsQuery(string doc_company, List<long> ticketIds);

        public string CreateGetPeopleLastChangeQuery(string doc_company);
        public string CreateGetPeopleByIdsQuery(string doc_company, List<string> ids);
        public string CreateDeletePersonQuery();
    }
}
