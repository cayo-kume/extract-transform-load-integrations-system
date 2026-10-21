namespace Application.Movidesk.Abstractions.Services
{
    public interface IMovideskPeopleService
    {
        /// <summary>
        /// busca uma pessoa pelo Cód. Ref., grava nas tabelas de pessoas e registra a chamada em general.MovideskRequests.
        /// true: gravada | false: a pessoa não existe na Movidesk | null: empresa não cadastrada ou resposta inválida da API
        /// </summary>
        public Task<bool?> GetPerson(string user_name, string doc_company, string id);

        public Task<bool?> GetPeople();
    }
}
