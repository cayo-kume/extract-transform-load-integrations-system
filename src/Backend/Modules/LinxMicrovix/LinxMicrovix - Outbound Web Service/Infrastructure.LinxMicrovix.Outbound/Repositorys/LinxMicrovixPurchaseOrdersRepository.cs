using Domain.LinxMicrovix.Outbound.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PurchaseOrders;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.LinxMicrovix.Outbound.Repositorys
{
    /// <summary>
    /// gravação dos métodos do LinxMicrovixPurchaseOrdersService: as respostas são planas, então cada método é uma tabela, com um bulk
    /// em untreated e uma merge em linxmicrovix (sem filhas, sem condição de exclusão)
    /// </summary>
    public class LinxMicrovixPurchaseOrdersRepository : ILinxMicrovixPurchaseOrdersRepository
    {
        private readonly ISharedRepository _coreRepository;

        public LinxMicrovixPurchaseOrdersRepository(ISharedRepository coreRepository) =>
            _coreRepository = coreRepository;

        public async Task<bool> BulkInsertIntoLinxPedidosCompra(List<LinxPedidosCompra> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxPedidosCompra(), tableName: "LinxPedidosCompra");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxPedidosCompraStatus(List<LinxPedidosCompraStatus> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxPedidosCompraStatus(), tableName: "LinxPedidosCompraStatus");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxPedidosCompraNumeroFornecedor(List<LinxPedidosCompraNumeroFornecedor> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxPedidosCompraNumeroFornecedor(), tableName: "LinxPedidosCompraNumeroFornecedor");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }
    }
}
