using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.PurchaseOrders;
using System.Collections;
using System.Data;

namespace Infrastructure.Totvs.Repositorys
{
    public class TotvsPurchaseOrdersRepository : ITotvsPurchaseOrdersRepository
    {
        private readonly ITotvsPurchaseOrdersCommandHandler _totvsPurchaseOrdersCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsPurchaseOrdersRepository(ITotvsPurchaseOrdersCommandHandler totvsPurchaseOrdersCommandHandler, ISharedRepository coreRepository) =>
            (_totvsPurchaseOrdersCommandHandler, _coreRepository) = (totvsPurchaseOrdersCommandHandler, coreRepository);

        public async Task<DateTime?> GetPurchaseOrdersLastChange(string doc_company)
        {
            string sql = _totvsPurchaseOrdersCommandHandler.CreateGetPurchaseOrdersLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<PurchaseOrder?>> GetPurchaseOrdersByKeys(string doc_company, List<(int branchCode, int orderCode)> keys)
        {
            var result = new List<PurchaseOrder?>();

            // lotes de 1000 chaves, como no GetPeopleByIds do Movidesk
            foreach (var chunk in keys.Distinct().Chunk(1000))
            {
                string sql = _totvsPurchaseOrdersCommandHandler.CreateGetPurchaseOrdersByKeysQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<PurchaseOrder>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsPurchaseOrders(List<PurchaseOrder> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: pedido, filhas (e netas logo depois da filha) (FKs e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new PurchaseOrder(), tableName: "TotvsPurchaseOrders"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Product(), tableName: "TotvsPurchaseOrdersProducts"), data.SelectMany(x => x.items).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Request(), tableName: "TotvsPurchaseOrdersRequests"), data.SelectMany(x => x.requests).ToList()),
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

        public async Task<string?> GetPurchaseOrderStatus(string doc_company, int branchCode, int orderCode)
        {
            string sql = _totvsPurchaseOrdersCommandHandler.CreateGetPurchaseOrderStatusQuery(doc_company, branchCode, orderCode);

            return await _coreRepository.GetRecord<string?>(sql);
        }

        public async Task<Domain.Totvs.Entities.Writes.PurchaseOrders.PurchaseOrderState?> GetPurchaseOrderState(string doc_company, int branchCode, int orderCode)
        {
            string sql = _totvsPurchaseOrdersCommandHandler.CreateGetPurchaseOrderStateQuery(doc_company, branchCode, orderCode);

            return (await _coreRepository.GetRecords<Domain.Totvs.Entities.Writes.PurchaseOrders.PurchaseOrderState>(sql)).FirstOrDefault();
        }

        public async Task<int?> GetPurchaseOrderBySupplierOrderCode(string doc_company, int branchCode, int? supplierCode, string? supplierCpfCnpj, string supplierOrderCode)
        {
            string sql = _totvsPurchaseOrdersCommandHandler.CreateGetPurchaseOrderBySupplierOrderCodeQuery(doc_company, branchCode, supplierCode, supplierCpfCnpj, supplierOrderCode);

            return await _coreRepository.GetRecord<int?>(sql);
        }

        public async Task<IEnumerable<Domain.Totvs.Entities.Writes.PurchaseOrders.OrderProductQuantity?>> GetPurchaseOrderProducts(string doc_company, int branchCode, int orderCode)
        {
            string sql = _totvsPurchaseOrdersCommandHandler.CreateGetPurchaseOrderProductsQuery(doc_company, branchCode, orderCode);

            return await _coreRepository.GetRecords<Domain.Totvs.Entities.Writes.PurchaseOrders.OrderProductQuantity>(sql);
        }
    }
}
