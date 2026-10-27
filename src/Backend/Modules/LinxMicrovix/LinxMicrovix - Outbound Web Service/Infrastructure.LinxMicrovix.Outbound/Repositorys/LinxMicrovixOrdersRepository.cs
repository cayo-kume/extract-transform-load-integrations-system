using Domain.LinxMicrovix.Outbound.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.LinxMicrovix.Outbound.Repositorys
{
    /// <summary>
    /// gravação dos métodos do LinxMicrovixOrdersService: as respostas são planas, então cada método é uma tabela, com um bulk
    /// em untreated e uma merge em linxmicrovix (sem filhas, sem condição de exclusão)
    /// </summary>
    public class LinxMicrovixOrdersRepository : ILinxMicrovixOrdersRepository
    {
        private readonly ISharedRepository _coreRepository;

        public LinxMicrovixOrdersRepository(ISharedRepository coreRepository) =>
            _coreRepository = coreRepository;

        public async Task<bool> BulkInsertIntoLinxPedidosVenda(List<LinxPedidosVenda> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxPedidosVenda(), tableName: "LinxPedidosVenda");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxPlanosPedidoVenda(List<LinxPlanosPedidoVenda> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxPlanosPedidoVenda(), tableName: "LinxPlanosPedidoVenda");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxPedidosVendaChecklistEntregaLocal(List<LinxPedidosVendaChecklistEntregaLocal> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxPedidosVendaChecklistEntregaLocal(), tableName: "LinxPedidosVendaChecklistEntregaLocal");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxPedidosVendaChecklistEntregaArmazenamento(List<LinxPedidosVendaChecklistEntregaArmazenamento> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxPedidosVendaChecklistEntregaArmazenamento(), tableName: "LinxPedidosVendaChecklistEntregaArmazenamento");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxPedidosVendaChecklistEntregaDificuldade(List<LinxPedidosVendaChecklistEntregaDificuldade> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxPedidosVendaChecklistEntregaDificuldade(), tableName: "LinxPedidosVendaChecklistEntregaDificuldade");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxOrcamentoPosicao(List<LinxOrcamentoPosicao> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxOrcamentoPosicao(), tableName: "LinxOrcamentoPosicao");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxB2CPedidos(List<LinxB2CPedidos> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxB2CPedidos(), tableName: "LinxB2CPedidos");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxB2CPedidosItens(List<LinxB2CPedidosItens> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxB2CPedidosItens(), tableName: "LinxB2CPedidosItens");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxB2CPedidosStatus(List<LinxB2CPedidosStatus> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxB2CPedidosStatus(), tableName: "LinxB2CPedidosStatus");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxB2CStatus(List<LinxB2CStatus> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxB2CStatus(), tableName: "LinxB2CStatus");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }
    }
}
