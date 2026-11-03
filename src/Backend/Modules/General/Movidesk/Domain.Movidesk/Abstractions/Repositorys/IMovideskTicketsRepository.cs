namespace Domain.Movidesk.Abstractions.Repositorys
{
    public interface IMovideskTicketsRepository
    {
        public Task<DateTime?> GetTicketsLastUpdate(string doc_company);

        /// <summary>
        /// marca d'água do GetTickets (movidesk.MovideskParameters.tickets_synced_until, UTC); nula antes da primeira execução completa
        /// </summary>
        public Task<DateTime?> GetTicketsSyncedUntil(string doc_company);

        public Task SetTicketsSyncedUntil(string doc_company, DateTime syncedUntil);
        public Task<IEnumerable<Domain.Movidesk.Entities.Reads.Tickets.Ticket?>> GetTicketsByIds(string doc_company, List<long> ids);
        public Task<IEnumerable<Domain.Movidesk.Entities.Reads.Tickets.Action?>> GetActionsHtmlDescriptions(string doc_company, List<long> ticketIds);
        public Task<bool> BulkInsertIntoMovideskTickets(List<Domain.Movidesk.Entities.Reads.Tickets.Ticket> data, Guid? parentExecutionGUID);
    }
}
