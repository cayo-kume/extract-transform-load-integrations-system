namespace Application.Movidesk.Abstractions.Services
{
    public interface IMovideskTicketsService
    {
        /// <summary>
        /// busca um ticket por número, grava nas tabelas de tickets e registra a chamada em movidesk.MovideskRequests.
        /// true: gravado | false: o ticket não existe na Movidesk | null: empresa não cadastrada ou resposta inválida da API
        /// </summary>
        public Task<bool?> GetTicketById(string user_name, string doc_company, long id);

        public Task<bool?> GetTickets();

        /// <summary>
        /// cria um ticket (POST tickets?returnAllProperties=true) e registra a chamada em movidesk.MovideskRequests.
        /// Se a API devolver o ticket inteiro, ele é gravado; senão, chega pela próxima execução do GetTickets.
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PostTicket(string user_name, string doc_company, Domain.Movidesk.Dtos.Writes.Tickets.Ticket ticket);

        /// <summary>
        /// altera um ticket (PATCH tickets?id={id}): só os campos enviados mudam, e listas enviadas substituem a lista inteira.
        /// A alteração chega ao banco pela próxima execução do GetTickets.
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PatchTicketById(string user_name, string doc_company, long id, Domain.Movidesk.Dtos.Writes.Tickets.Ticket ticket);

        /// <summary>
        /// envia anexos para uma ação (POST ticketFileUpload?id={id}&amp;actionId={actionId}), todos numa chamada
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PostTicketFileUploadById(string user_name, string doc_company, long id, int actionId, IReadOnlyList<(string fileName, string contentType, byte[] content)> files);
    }
}
