using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.Core.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxMicrovix
{
    public class LinxProdutosDetalhesRepository : ILinxProdutosDetalhesRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly ILinxProdutosDetalhesCommandHandler _commandHandler;

        public LinxProdutosDetalhesRepository(IIntegrationsCoreRepository coreRepository, ILinxProdutosDetalhesCommandHandler commandHandler)
        {
            _coreRepository = coreRepository;
            _commandHandler = commandHandler;
        }

        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxProdutosDetalhes> records)
        {
            var table = _coreRepository.CreateSystemDataTable(jobParameter.tableName, new LinxProdutosDetalhes());

            _coreRepository.FillSystemDataTable(table, records.ToList());

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: table
            );

            return true;
        }

        public async Task<IEnumerable<LinxProdutosDetalhes>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxProdutosDetalhes> registros)
        {
            string sql = _commandHandler.CreateGetRegistersExistsQuery(registros);
            return await _coreRepository.GetRecords<LinxProdutosDetalhes>(sql);
        }

        public async Task<IEnumerable<LinxProdutosDetalhes>> GetRegistersExists(LinxAPIParam jobParameter)
        {
            string sql = _commandHandler.CreateGetPendingRegistersQuery();
            return await _coreRepository.GetRecords<LinxProdutosDetalhes>(sql);
        }

        public async Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxProdutosDetalhes? record)
        {
            string sql = _commandHandler.CreateInsertRecordQuery(jobParameter.tableName);
            return await _coreRepository.InsertRecord(sql: sql, entity: record);
        }
    }
}
