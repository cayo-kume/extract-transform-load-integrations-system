namespace Application.Movidesk.Abstractions.Services
{
    public interface IMovideskTicketsService
    {
        /// <summary>
        /// busca um ticket por número, grava nas tabelas de tickets e registra a chamada em general.MovideskRequests.
        /// true: gravado | false: o ticket não existe na Movidesk | null: empresa não cadastrada ou resposta inválida da API
        /// </summary>
        public Task<bool?> GetTicket(string user_name, string doc_company, long id);

        public Task<bool?> GetTickets();
    }
}
