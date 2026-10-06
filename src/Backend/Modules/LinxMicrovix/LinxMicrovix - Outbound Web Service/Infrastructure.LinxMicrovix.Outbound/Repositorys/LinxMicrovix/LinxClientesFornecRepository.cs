using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.Core.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxMicrovix
{
    public class LinxClientesFornecRepository : ILinxClientesFornecRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly ILinxClientesFornecCommandHandler _commandHandler;

        public LinxClientesFornecRepository(IIntegrationsCoreRepository coreRepository, ILinxClientesFornecCommandHandler commandHandler)
        {
            _coreRepository = coreRepository;
            _commandHandler = commandHandler;
        }

        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxClientesFornec> records)
        {
            var table = _coreRepository.CreateSystemDataTable(jobParameter.tableName, new LinxClientesFornec());

            _coreRepository.FillSystemDataTable(table, records.ToList());

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: table
            );

            return true;
        }

        public async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<string?>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();
                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetRegistersExistsQuery(batch);
                    var result = await _coreRepository.GetRecords<string>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }

        public async Task<bool> InsertRecord(LinxAPIParam jobParameter, LinxClientesFornec? record)
        {
            string sql = _commandHandler.CreateInsertRecordQuery(jobParameter.tableName);
            return await _coreRepository.InsertRecord(sql: sql, entity: record);
        }

        public async Task<bool> InsertRecord(string identifier, bool is_present_in_erp)
        {
            string sql = _commandHandler.CreateInsertRecordQuery(identifier: identifier, is_present_in_erp: is_present_in_erp);
            return await _coreRepository.ExecuteCommand(sql: sql);
        }
    }
}
