using Domain.Movidesk.Entities.Parameters;

namespace Domain.Movidesk.Abstractions.Repositorys
{
    public interface IMovideskRepository
    {
        public Task<IEnumerable<Company?>> GetCompanys();
        public Task<Company?> GetCompany(string doc_company);

        public bool InsertIntoMovideskRequests(Domain.Movidesk.Entities.Writes.Requests.Request request);
    }
}
