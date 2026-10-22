namespace Domain.Movidesk.Abstractions.Repositorys
{
    public interface IMovideskPeopleRepository
    {
        public Task<DateTime?> GetPeopleLastChange(string doc_company);
        public Task<IEnumerable<Domain.Movidesk.Entities.Reads.People.Person?>> GetPeopleByIds(string doc_company, List<string> ids);
        public Task<bool> BulkInsertIntoMovideskPeople(List<Domain.Movidesk.Entities.Reads.People.Person> data, Guid? parentExecutionGUID);

        /// <summary>
        /// apaga a pessoa e as filhas do banco local (depois da exclusão na Movidesk)
        /// </summary>
        public Task<bool> DeletePerson(string doc_company, string id);
    }
}
