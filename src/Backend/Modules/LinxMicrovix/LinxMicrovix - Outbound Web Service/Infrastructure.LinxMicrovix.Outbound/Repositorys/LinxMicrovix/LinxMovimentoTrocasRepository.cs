using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.Core.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxMicrovix
{
    public class LinxMovimentoTrocasRepository : ILinxMovimentoTrocasRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly ILinxMovimentoTrocasCommandHandler _commandHandler;

        public LinxMovimentoTrocasRepository(IIntegrationsCoreRepository coreRepository, ILinxMovimentoTrocasCommandHandler commandHandler)
        {
            _coreRepository = coreRepository;
            _commandHandler = commandHandler;
        }

        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxMovimentoTrocas> records)
        {
            var table = _coreRepository.CreateSystemDataTable(jobParameter.tableName, new LinxMovimentoTrocas());

            _coreRepository.FillSystemDataTable(table, records.ToList());

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: table
            );

            return true;
        }

        public async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxMovimentoTrocas?> registros)
        {
            string sql = _commandHandler.CreateGetRegistersExistsQuery(registros);
            return await _coreRepository.GetRecords<string>(sql);
        }

        public async Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxMovimentoTrocas? record)
        {
            string sql = _commandHandler.CreateInsertRecordQuery(jobParameter.tableName);
            return await _coreRepository.InsertRecord(sql: sql, entity: record);
        }
    }
}
