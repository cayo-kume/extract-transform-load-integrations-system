using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.SalesOrder;

namespace Infrastructure.Totvs.Repositorys
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly ISalesOrderCommandHandler _commandHandler;
        private readonly IIntegrationsSharedRepository _coreRepository;

        public SalesOrderRepository(IIntegrationsSharedRepository coreRepository, ISalesOrderCommandHandler commandHandler) =>
            (_coreRepository, _commandHandler) = (coreRepository, commandHandler);

        public async Task<bool> BulkInsertIntoTableSalesOrdersRaw(List<SaleOrder> registros, Guid? parentExecutionGUID)
        {
            var salesOrdersTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.SaleOrder(), tableName: "SaleOrders");
            var productsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.Product(), tableName: "SaleOrdersProducts");
            var productsBatchTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.BatchItem(), tableName: "SaleOrdersProductsBatchsItems");
            var invoicesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.Invoice(), tableName: "SaleOrdersInvoices");
            var observationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.Observation(), tableName: "SaleOrdersObservations");
            var invoicesObservationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.InvoiceObservation(), tableName: "SaleOrdersInvoiceObservations");
            var representativeObservationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.RepresentativeObservation(), tableName: "SaleOrdersRepresentativeObservations");
            var classificationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.Classification(), tableName: "SaleOrdersClassifications");
            var discountsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.Discount(), tableName: "SaleOrdersDiscounts");
            var commissionedsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.Commissioned(), tableName: "SaleOrdersCommissioneds");
            var countsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.SalesOrder.Count(), tableName: "SaleOrdersCounts");

            _coreRepository.FillSystemDataTable(salesOrdersTable, registros.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(productsTable, registros.Select(y => y.items).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(productsBatchTable, registros.Select(y => y.items).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList().Select(w => w.batchItems).Where(h => h.Count > 0).Select(a => a.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(invoicesTable, registros.Select(y => y.invoices).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(observationsTable, registros.Select(y => y.observations).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(invoicesObservationsTable, registros.Select(y => y.invoiceObservations).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(representativeObservationsTable, registros.Select(y => y.representativeObservations).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(classificationsTable, registros.Select(y => y.classifications).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(discountsTable, registros.Select(y => y.discounts).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(commissionedsTable, registros.Select(y => y.commissioneds).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(countsTable, registros.Select(y => y.counts).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());

            _coreRepository.BulkInsertIntoTableRaw(salesOrdersTable);
            _coreRepository.BulkInsertIntoTableRaw(productsTable);
            _coreRepository.BulkInsertIntoTableRaw(productsBatchTable);
            _coreRepository.BulkInsertIntoTableRaw(invoicesTable);
            _coreRepository.BulkInsertIntoTableRaw(observationsTable);
            _coreRepository.BulkInsertIntoTableRaw(invoicesObservationsTable);
            _coreRepository.BulkInsertIntoTableRaw(representativeObservationsTable);
            _coreRepository.BulkInsertIntoTableRaw(classificationsTable);
            _coreRepository.BulkInsertIntoTableRaw(discountsTable);
            _coreRepository.BulkInsertIntoTableRaw(commissionedsTable);
            _coreRepository.BulkInsertIntoTableRaw(countsTable);

            await _coreRepository.CallDbProcMerge("totvs", salesOrdersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsBatchTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", invoicesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", observationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", invoicesObservationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", representativeObservationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", classificationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", discountsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", commissionedsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", countsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.Totvs.Dtos.SalesOrder.Item?>> GetExistingSalesOrders(List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<Domain.Totvs.Dtos.SalesOrder.Item>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetExistingSalesOrdersQuery(batch);
                    var result = await _coreRepository.GetRecords<Domain.Totvs.Dtos.SalesOrder.Item>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }
    }
}
