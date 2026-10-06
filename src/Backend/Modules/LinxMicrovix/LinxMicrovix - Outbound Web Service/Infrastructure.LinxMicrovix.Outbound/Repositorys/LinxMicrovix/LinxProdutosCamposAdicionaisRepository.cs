using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.Core.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxMicrovix
{
    public class LinxProdutosCamposAdicionaisRepository : ILinxProdutosCamposAdicionaisRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly ILinxProdutosCamposAdicionaisCommandHandler _commandHandler;

        public LinxProdutosCamposAdicionaisRepository(IIntegrationsCoreRepository coreRepository, ILinxProdutosCamposAdicionaisCommandHandler commandHandler)
        {
            _coreRepository = coreRepository;
            _commandHandler = commandHandler;
        }

        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxProdutosCamposAdicionais> records)
        {
            var table = _coreRepository.CreateSystemDataTable(jobParameter.tableName, new LinxProdutosCamposAdicionais());

            _coreRepository.FillSystemDataTable(table, records.ToList());

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: table
            );

            return true;
        }

        public async Task<IEnumerable<string>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxProdutosCamposAdicionais> registros)
        {
            string sql = _commandHandler.CreateGetRegistersExistsQuery(registros);
            return await _coreRepository.GetRecords<string>(sql);
        }

        public async Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxProdutosCamposAdicionais? record)
        {
            string sql = _commandHandler.CreateInsertRecordQuery(jobParameter.tableName);
            return await _coreRepository.InsertRecord(sql: sql, entity: record);
        }
    }
}
