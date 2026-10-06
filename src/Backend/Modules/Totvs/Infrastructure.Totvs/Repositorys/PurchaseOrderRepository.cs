using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Dtos.PurchaseOrder;
using Domain.Totvs.Entities.PurchaseOrder;

namespace Infrastructure.Totvs.Repositorys
{
    internal class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly IPurchaseOrderCommandHandler _commandHandler;
        private readonly IIntegrationsSharedRepository _coreRepository;

        public PurchaseOrderRepository(IIntegrationsSharedRepository coreRepository, IPurchaseOrderCommandHandler commandHandler) =>
            (_coreRepository, _commandHandler) = (coreRepository, commandHandler);

        public async Task<bool> BulkInsertIntoTablePurchaseOrdersRaw(List<PurchaseOrder> registros, Guid? parentExecutionGUID)
        {
            var purchaseOrdersTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.PurchaseOrder.PurchaseOrder(), tableName: "PurchaseOrders");
            var productsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.PurchaseOrder.Product(), tableName: "PurchaseOrdersProducts");

            _coreRepository.FillSystemDataTable(purchaseOrdersTable, registros.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(productsTable, registros.Select(y => y.items).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());

            _coreRepository.BulkInsertIntoTableRaw(purchaseOrdersTable);
            _coreRepository.BulkInsertIntoTableRaw(productsTable);

            await _coreRepository.CallDbProcMerge("totvs", purchaseOrdersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", productsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Item?>> GetExistingPurchaseOrders(List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<Domain.Totvs.Dtos.PurchaseOrder.Item>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetExistingPurchaseOrdersQuery(batch);
                    var result = await _coreRepository.GetRecords<Domain.Totvs.Dtos.PurchaseOrder.Item>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }
    }
}
