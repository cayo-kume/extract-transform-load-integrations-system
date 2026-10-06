using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.Core.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxMicrovix
{
    public class LinxUsuariosRepository : ILinxUsuariosRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly ILinxUsuariosCommandHandler _commandHandler;

        public LinxUsuariosRepository(IIntegrationsCoreRepository coreRepository, ILinxUsuariosCommandHandler commandHandler)
        {
            _coreRepository = coreRepository;
            _commandHandler = commandHandler;
        }

        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<LinxUsuarios> records)
        {
            var table = _coreRepository.CreateSystemDataTable(jobParameter.tableName, new LinxUsuarios());
            _coreRepository.FillSystemDataTable(table, records.ToList());
            _coreRepository.BulkInsertIntoTableRaw(dataTable: table);
            return true;
        }

        public async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<int?> registros)
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
    }
}
