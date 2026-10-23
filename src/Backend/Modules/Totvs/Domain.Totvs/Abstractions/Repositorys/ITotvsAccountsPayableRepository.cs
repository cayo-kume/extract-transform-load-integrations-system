using Domain.Totvs.Entities.Reads.AccountsPayable;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsAccountsPayableRepository
    {
        public Task<DateTime?> GetDuplicatesLastChange(string doc_company);
        public Task<IEnumerable<Duplicate?>> GetDuplicatesByKeys(string doc_company, List<(int branchCode, int supplierCode, long duplicateCode, int installmentCode)> keys);
        public Task<bool> BulkInsertIntoTotvsDuplicates(List<Duplicate> data, Guid? parentExecutionGUID);

        public Task<bool> BulkInsertIntoTotvsCommissionsPaid(List<CommissionPaid> data, Guid? parentExecutionGUID);
    }
}
