using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.Invoices;
using System.Collections;
using System.Data;

namespace Infrastructure.Totvs.Repositorys
{
    public class TotvsInvoicesRepository : ITotvsInvoicesRepository
    {
        private readonly ITotvsInvoicesCommandHandler _totvsInvoicesCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsInvoicesRepository(ITotvsInvoicesCommandHandler totvsInvoicesCommandHandler, ISharedRepository coreRepository) =>
            (_totvsInvoicesCommandHandler, _coreRepository) = (totvsInvoicesCommandHandler, coreRepository);

        public async Task<DateTime?> GetInvoicesLastChange(string doc_company)
        {
            string sql = _totvsInvoicesCommandHandler.CreateGetInvoicesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Invoice?>> GetInvoicesByKeys(string doc_company, List<(int branchCode, int invoiceSequence, DateTime invoiceDate)> keys)
        {
            var result = new List<Invoice?>();

            // lotes de 1000 chaves, como no GetPeopleByIds do Movidesk
            foreach (var chunk in keys.Distinct().Chunk(1000))
            {
                string sql = _totvsInvoicesCommandHandler.CreateGetInvoicesByKeysQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Invoice>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsInvoices(List<Invoice> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: nota, filhas (e netas logo depois da filha) (FKs e condição de exclusão do merge)
            var items = data.SelectMany(x => x.items).ToList();
            var products = items.SelectMany(x => x.products).ToList();

            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Invoice(), tableName: "TotvsInvoices"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Item(), tableName: "TotvsInvoicesItems"), items),
                (_coreRepository.CreateSystemDataTable(entity: new ItemProduct(), tableName: "TotvsInvoicesItemsProducts"), products),
                (_coreRepository.CreateSystemDataTable(entity: new ItemProductReferencedInvoice(), tableName: "TotvsInvoicesItemsProductsReferenced"), products.SelectMany(x => x.referencedInvoices).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ItemTax(), tableName: "TotvsInvoicesItemsTaxes"), items.SelectMany(x => x.taxes).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ItemBarcode(), tableName: "TotvsInvoicesItemsBarCodes"), items.SelectMany(x => x.barCodes).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Payment(), tableName: "TotvsInvoicesPayments"), data.SelectMany(x => x.payments).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new SalesOrder(), tableName: "TotvsInvoicesSalesOrders"), data.SelectMany(x => x.salesOrder).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ObservationNF(), tableName: "TotvsInvoicesObservationsNF"), data.SelectMany(x => x.observationNF).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new ReferencedTaxInvoice(), tableName: "TotvsInvoicesReferencedTaxes"), data.SelectMany(x => x.referencedTaxInvoice).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram da nota
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetInvoicesDisableLastChange(string doc_company)
        {
            string sql = _totvsInvoicesCommandHandler.CreateGetInvoicesDisableLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<InvoiceDisable?>> GetInvoicesDisable(string doc_company)
        {
            string sql = _totvsInvoicesCommandHandler.CreateGetInvoicesDisableQuery(doc_company);

            return await _coreRepository.GetRecords<InvoiceDisable>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsInvoicesDisable(List<InvoiceDisable> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new InvoiceDisable(), tableName: "TotvsInvoicesDisable");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<IEnumerable<CostCenter?>> GetCostCentersHashes(string doc_company)
        {
            string sql = _totvsInvoicesCommandHandler.CreateGetCostCentersHashesQuery(doc_company);

            return await _coreRepository.GetRecords<CostCenter>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsCostCenters(List<CostCenter> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: o centro antes das classificações (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new CostCenter(), tableName: "TotvsCostCenters"), data),
                (_coreRepository.CreateSystemDataTable(entity: new CostCenterClassification(), tableName: "TotvsCostCentersClassifications"), data.SelectMany(x => x.classifications).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das classificações roda mesmo sem linhas no lote: é ele que apaga as que saíram do centro
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
