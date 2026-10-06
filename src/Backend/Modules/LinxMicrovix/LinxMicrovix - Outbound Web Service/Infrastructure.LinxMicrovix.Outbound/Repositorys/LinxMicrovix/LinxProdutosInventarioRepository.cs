using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.Core.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxMicrovix
{
    public class LinxProdutosInventarioRepository : ILinxProdutosInventarioRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly ILinxProdutosInventarioCommandHandler _commandHandler;

        public LinxProdutosInventarioRepository(IIntegrationsCoreRepository coreRepository, ILinxProdutosInventarioCommandHandler commandHandler)
        {
            _coreRepository = coreRepository;
            _commandHandler = commandHandler;
        }

        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxProdutosInventario> records)
        {
            var table = _coreRepository.CreateSystemDataTable(jobParameter.tableName, new LinxProdutosInventario());

            _coreRepository.FillSystemDataTable(table, records.ToList());

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: table
            );

            return true;
        }

        public async Task<IEnumerable<string?>> GetProductsDepositsIds(LinxAPIParam jobParameter)
        {
            string sql = _commandHandler.CreateGetProductsDepositsIdsQuery();
            return await _coreRepository.GetRecords<string>(sql);
        }

        public async Task<IEnumerable<LinxProdutosInventario?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxProdutosInventario> registros)
        {
            string sql = _commandHandler.CreateGetRegistersExistsQuery(registros);
            return await _coreRepository.GetRecords<LinxProdutosInventario>(sql);
        }

        public async Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxProdutosInventario? record)
        {
            string sql = _commandHandler.CreateInsertRecordQuery(jobParameter.tableName);
            return await _coreRepository.InsertRecord(sql: sql, entity: record);
        }
    }
}
