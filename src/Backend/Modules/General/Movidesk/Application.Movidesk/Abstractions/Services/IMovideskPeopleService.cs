namespace Application.Movidesk.Abstractions.Services
{
    public interface IMovideskPeopleService
    {
        /// <summary>
        /// busca uma pessoa pelo Cód. Ref., grava nas tabelas de pessoas e registra a chamada em general.MovideskRequests.
        /// true: gravada | false: a pessoa não existe na Movidesk | null: empresa não cadastrada ou resposta inválida da API
        /// </summary>
        public Task<bool?> GetPersonById(string user_name, string doc_company, string id);

        public Task<bool?> GetPeople();

        /// <summary>
        /// cria uma pessoa (POST persons?returnAllProperties=true) e registra a chamada em general.MovideskRequests.
        /// Se a API devolver a pessoa inteira, ela é gravada; senão, chega pela próxima execução do GetPeople.
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PostPerson(string user_name, string doc_company, Domain.Movidesk.Dtos.Writes.People.Person person);

        /// <summary>
        /// altera uma pessoa (PATCH persons?id={id}): só os campos enviados mudam, e listas enviadas substituem a lista inteira.
        /// A alteração chega ao banco pela próxima execução do GetPeople.
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PatchPersonById(string user_name, string doc_company, string id, Domain.Movidesk.Dtos.Writes.People.Person person);

        /// <summary>
        /// inclui organizações numa pessoa (POST persons/relationships?id={id}) sem mexer nas que ela já tem
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> PostPersonRelationshipsById(string user_name, string doc_company, string id, Domain.Movidesk.Dtos.Writes.People.Relationships relationships);

        /// <summary>
        /// exclui uma pessoa (DELETE persons?id={id}), irreversível, e apaga a pessoa e as filhas do banco local
        /// </summary>
        public Task<Domain.Movidesk.Dtos.Writes.Result> DeletePersonById(string user_name, string doc_company, string id);
    }
}
