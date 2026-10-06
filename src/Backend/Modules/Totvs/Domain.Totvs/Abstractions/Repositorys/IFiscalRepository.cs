using Domain.Totvs.Entities.Fiscal;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface IFiscalRepository
    {
        public Task<List<Domain.Totvs.Dtos.Fiscal.Item?>> GetExistingInvoices(List<string?> registros);

        public Task<bool> BulkInsertIntoTableInvoicesRaw(List<Invoice> registros, Guid? parentExecutionGUID);
    }
}
