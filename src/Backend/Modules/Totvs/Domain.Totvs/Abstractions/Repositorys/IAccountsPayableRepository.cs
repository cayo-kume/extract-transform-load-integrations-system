using Domain.Totvs.Entities.AccountsPayable;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface IAccountsPayableRepository
    {
        public Task<bool> BulkInsertIntoTableDuplicatesRaw(List<Duplicate> registros, Guid? parentExecutionGUID);
        public Task<List<Domain.Totvs.Dtos.AccountsPayable.Duplicate?>> GetExistingDuplicates(List<string?> registros);

        public Task<bool> BulkInsertIntoTableComissionsPaidRaw(List<ComissionPaid> registros, Guid? parentExecutionGUID);
        public Task<List<Domain.Totvs.Dtos.AccountsPayable.ComissionPaid?>> GetExistingComissionsPaid(List<string?> registros);
    }
}
