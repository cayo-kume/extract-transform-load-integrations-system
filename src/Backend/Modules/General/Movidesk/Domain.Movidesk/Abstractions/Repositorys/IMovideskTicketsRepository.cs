namespace Domain.Movidesk.Abstractions.Repositorys
{
    public interface IMovideskTicketsRepository
    {
        public Task<DateTime?> GetTicketsLastUpdate(string doc_company);
        public Task<IEnumerable<Domain.Movidesk.Entities.Reads.Tickets.Ticket?>> GetTicketsByIds(string doc_company, List<long> ids);
        public Task<IEnumerable<Domain.Movidesk.Entities.Reads.Tickets.Action?>> GetActionsHtmlDescriptions(string doc_company, List<long> ticketIds);
        public Task<bool> BulkInsertIntoMovideskTickets(List<Domain.Movidesk.Entities.Reads.Tickets.Ticket> data, Guid? parentExecutionGUID);
    }
}
