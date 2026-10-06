using Domain.Movidesk.Entities;
//using Domain.Movidesk.Models.Tickets;

namespace Domain.Movidesk.Interfaces.Repositorys
{
    public interface IMovideskRepository
    {
        public Task<Parameters?> GetTokenAsync();

        //public Task<bool> BulkInsertIntoTableRaw(List<Domain.Movidesk.Models.Persons.Person> registros, Guid? parentExecutionGUID);
        //public Task<bool> BulkInsertIntoTableRaw(List<Domain.Movidesk.Models.Tickets.Ticket> registros, Guid? parentExecutionGUID);
        //public Task<bool> BulkInsertIntoTableRaw(List<Domain.Movidesk.Models.> registros, Guid? parentExecutionGUID);

        /*************************************************************************************************************************************************************************/

        //public Task<List<TicketAntigo?>> GetBlankTicketsFromDatabase();
        //public Task<List<TicketAntigo?>> GetTicketsFromDatabase();
        //public Task<bool> InsertInvoice(Invoice invoice);
        //public Task<bool> InsertTicket(TicketAntigo? ticket);
        //public Task UpdateTicketFromDatabase(List<TicketAntigo> tickets);
        //public Task UpdateTicketStatusFromDatabase(Invoice invoice);
    }
}
