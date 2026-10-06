using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Dtos.Fiscal;

namespace Infrastructure.Totvs.Repositorys
{
    public class FiscalRepository : IFiscalRepository
    {
        private readonly IFiscalCommandHandler _commandHandler;
        private readonly IIntegrationsSharedRepository _coreRepository;

        public FiscalRepository(IIntegrationsSharedRepository coreRepository, IFiscalCommandHandler commandHandler) =>
            (_coreRepository, _commandHandler) = (coreRepository, commandHandler);

        public async Task<bool> BulkInsertIntoTableInvoicesRaw(List<Domain.Totvs.Entities.Fiscal.Invoice> registros, Guid? parentExecutionGUID)
        {
            var invoicesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Fiscal.Invoice(), tableName: "Invoices");
            var paymentsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Fiscal.Payment(), tableName: "InvoicesPayments");
            var salesOrdersTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Fiscal.SaleOrder(), tableName: "InvoicesSalesOrders");
            var referencedTaxesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Fiscal.ReferencedTax(), tableName: "InvoicesReferencedTaxes");
            var observationsNFTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Fiscal.ObservationNF(), tableName: "InvoicesObservationsNF");
            var itemsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Fiscal.ProductItem(), tableName: "InvoicesItems");
            var itemsTaxesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Fiscal.Tax(), tableName: "InvoicesItemsTaxes");
            var itemsProductsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Fiscal.Product(), tableName: "InvoicesItemsProducts");
            var itemsBarcodesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Fiscal.BarCode(), tableName: "InvoicesItemsBarCodes");
            var itemsProductsReferencedTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Fiscal.ReferencedInvoice(), tableName: "InvoicesItemsProductsReferenced");

            _coreRepository.FillSystemDataTable(invoicesTable, registros.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(paymentsTable, registros.Select(y => y.payments).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(salesOrdersTable, registros.Select(y => y.salesOrder).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(referencedTaxesTable, registros.Select(y => y.referencedTaxInvoice).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(observationsNFTable, registros.Select(y => y.observationNF).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(itemsTable, registros.Select(y => y.items).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(itemsTaxesTable, registros.Select(y => y.items).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList().Select(w => w.taxes).Where(h => h.Count > 0).Select(a => a.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(itemsProductsTable, registros.Select(y => y.items).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList().Select(w => w.products).Where(h => h.Count > 0).Select(a => a.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(itemsBarcodesTable, registros.Select(y => y.items).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList().Select(w => w.barCodes).Where(h => h.Count > 0).Select(a => a.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(itemsProductsReferencedTable, registros.Select(y => y.items).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList().Select(w => w.products).Where(h => h.Count > 0).Select(a => a.FirstOrDefault()).ToList().Select(y => y.referencedInvoices).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());

            _coreRepository.BulkInsertIntoTableRaw(invoicesTable);
            _coreRepository.BulkInsertIntoTableRaw(paymentsTable);
            _coreRepository.BulkInsertIntoTableRaw(salesOrdersTable);
            _coreRepository.BulkInsertIntoTableRaw(referencedTaxesTable);
            _coreRepository.BulkInsertIntoTableRaw(observationsNFTable);
            _coreRepository.BulkInsertIntoTableRaw(itemsTable);
            _coreRepository.BulkInsertIntoTableRaw(itemsTaxesTable);
            _coreRepository.BulkInsertIntoTableRaw(itemsProductsTable);
            _coreRepository.BulkInsertIntoTableRaw(itemsBarcodesTable);
            _coreRepository.BulkInsertIntoTableRaw(itemsProductsReferencedTable);

            await _coreRepository.CallDbProcMerge("totvs", invoicesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", paymentsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", salesOrdersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", referencedTaxesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", observationsNFTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", itemsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", itemsTaxesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", itemsProductsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", itemsBarcodesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", itemsProductsReferencedTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Item?>> GetExistingInvoices(List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<Domain.Totvs.Dtos.Fiscal.Item>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetExistingInvoicesQuery(batch);
                    var result = await _coreRepository.GetRecords<Domain.Totvs.Dtos.Fiscal.Item>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }
    }
}
