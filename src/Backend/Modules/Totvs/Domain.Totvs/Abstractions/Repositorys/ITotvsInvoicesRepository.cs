using Domain.Totvs.Entities.Reads.Invoices;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsInvoicesRepository
    {
        public Task<DateTime?> GetInvoicesLastChange(string doc_company);
        public Task<IEnumerable<Invoice?>> GetInvoicesByKeys(string doc_company, List<(int branchCode, int invoiceSequence, DateTime invoiceDate)> keys);
        public Task<bool> BulkInsertIntoTotvsInvoices(List<Invoice> data, Guid? parentExecutionGUID);
    }
}
