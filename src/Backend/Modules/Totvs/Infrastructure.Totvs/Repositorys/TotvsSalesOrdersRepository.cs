using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.SalesOrders;
using System.Collections;
using System.Data;

namespace Infrastructure.Totvs.Repositorys
{
    public class TotvsSalesOrdersRepository : ITotvsSalesOrdersRepository
    {
        private readonly ITotvsCommandHandler _totvsCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsSalesOrdersRepository(ITotvsCommandHandler totvsCommandHandler, ISharedRepository coreRepository) =>
            (_totvsCommandHandler, _coreRepository) = (totvsCommandHandler, coreRepository);

        public async Task<DateTime?> GetSalesOrdersLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetSalesOrdersLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<SaleOrder?>> GetSalesOrdersByKeys(string doc_company, List<(int branchCode, int orderCode)> keys)
        {
            var result = new List<SaleOrder?>();

            // lotes de 1000 chaves, como no GetPeopleByIds do Movidesk
            foreach (var chunk in keys.Distinct().Chunk(1000))
            {
                string sql = _totvsCommandHandler.CreateGetSalesOrdersByKeysQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<SaleOrder>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsSaleOrders(List<SaleOrder> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: pedido, filhas (e netas logo depois da filha) (FKs e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new SaleOrder(), tableName: "TotvsSaleOrders"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Product(), tableName: "TotvsSaleOrdersProducts"), data.SelectMany(x => x.items).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new BatchItem(), tableName: "TotvsSaleOrdersProductsBatchItems"), data.SelectMany(x => x.items).SelectMany(x => x.batchItems).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Invoice(), tableName: "TotvsSaleOrdersInvoices"), data.SelectMany(x => x.invoices).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Observation(), tableName: "TotvsSaleOrdersObservations"), data.SelectMany(x => x.observations).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new InvoiceObservation(), tableName: "TotvsSaleOrdersInvoiceObservations"), data.SelectMany(x => x.invoiceObservations).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new RepresentativeObservation(), tableName: "TotvsSaleOrdersRepresentativeObservations"), data.SelectMany(x => x.representativeObservations).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Classification(), tableName: "TotvsSaleOrdersClassifications"), data.SelectMany(x => x.classifications).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Discount(), tableName: "TotvsSaleOrdersDiscounts"), data.SelectMany(x => x.discounts).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Commissioned(), tableName: "TotvsSaleOrdersCommissioneds"), data.SelectMany(x => x.commissioneds).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Count(), tableName: "TotvsSaleOrdersCounts"), data.SelectMany(x => x.counts).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do pedido
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetSaleDiscountTypesLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetSaleDiscountTypesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleDiscountType?>> GetSaleDiscountTypes(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetSaleDiscountTypesQuery(doc_company);

            return await _coreRepository.GetRecords<Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleDiscountType>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsSaleDiscountTypes(List<Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleDiscountType> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleDiscountType(), tableName: "TotvsSaleDiscountTypes");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetSaleClassificationsLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetSaleClassificationsLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleClassification?>> GetSaleClassifications(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetSaleClassificationsQuery(doc_company);

            return await _coreRepository.GetRecords<Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleClassification>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsSaleClassifications(List<Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleClassification> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleClassification(), tableName: "TotvsSaleClassifications");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetSaleOrderBatchesLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetSaleOrderBatchesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleOrderBatch?>> GetSaleOrderBatches(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetSaleOrderBatchesQuery(doc_company);

            return await _coreRepository.GetRecords<Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleOrderBatch>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsSaleOrderBatches(List<Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleOrderBatch> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.SalesOrders.Catalogs.SaleOrderBatch(), tableName: "TotvsSaleOrderBatches");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetBillingSuggestionsLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetBillingSuggestionsLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestion?>> GetBillingSuggestionsByKeys(string doc_company, List<(int branchCode, int suggestionCode)> keys)
        {
            var result = new List<Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestion?>();

            // lotes de 1000 chaves, como nos pedidos
            foreach (var chunk in keys.Distinct().Chunk(1000))
            {
                string sql = _totvsCommandHandler.CreateGetBillingSuggestionsByKeysQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestion>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsBillingSuggestions(List<Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestion> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: sugestão, pedidos e itens (FKs e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestion(), tableName: "TotvsBillingSuggestions"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestionOrder(), tableName: "TotvsBillingSuggestionsOrders"), data.SelectMany(x => x.orders).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.SalesOrders.BillingSuggestions.BillingSuggestionOrderItem(), tableName: "TotvsBillingSuggestionsOrdersItems"), data.SelectMany(x => x.orderItems).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram da sugestão
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
