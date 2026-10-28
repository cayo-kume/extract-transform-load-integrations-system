using Domain.Totvs.Entities.Reads.Invoices;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsInvoicesRepository
    {
        public Task<DateTime?> GetInvoicesLastChange(string doc_company);
        public Task<IEnumerable<Invoice?>> GetInvoicesByKeys(string doc_company, List<(int branchCode, int invoiceSequence, DateTime invoiceDate)> keys);
        public Task<bool> BulkInsertIntoTotvsInvoices(List<Invoice> data, Guid? parentExecutionGUID);
        public Task<DateTime?> GetInvoicesDisableLastChange(string doc_company);
        public Task<IEnumerable<InvoiceDisable?>> GetInvoicesDisable(string doc_company);
        public Task<bool> BulkInsertIntoTotvsInvoicesDisable(List<InvoiceDisable> data, Guid? parentExecutionGUID);
        public Task<IEnumerable<CostCenter?>> GetCostCentersHashes(string doc_company);
        public Task<bool> BulkInsertIntoTotvsCostCenters(List<CostCenter> data, Guid? parentExecutionGUID);
    }
}
