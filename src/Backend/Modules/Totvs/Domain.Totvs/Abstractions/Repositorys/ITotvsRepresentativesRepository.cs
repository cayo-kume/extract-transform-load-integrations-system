using Domain.Totvs.Entities.Reads.People.ReceptionSales;
using Domain.Totvs.Entities.Reads.People.Representatives;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsRepresentativesRepository
    {
        public Task<DateTime?> GetRepresentativesLastChange(string doc_company);
        public Task<IEnumerable<Representative?>> GetRepresentativesByCodes(string doc_company, List<int> codes);
        public Task<bool> BulkInsertIntoTotvsRepresentatives(List<Representative> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetReceptionSalesLastMovementDate(string doc_company);
        public Task<IEnumerable<ReceptionSale?>> GetReceptionSalesHashes(string doc_company, DateTime startMovementDate, DateTime endMovementDate);
        public Task<bool> BulkInsertIntoTotvsReceptionSales(List<ReceptionSale> data, Guid? parentExecutionGUID);
    }
}
