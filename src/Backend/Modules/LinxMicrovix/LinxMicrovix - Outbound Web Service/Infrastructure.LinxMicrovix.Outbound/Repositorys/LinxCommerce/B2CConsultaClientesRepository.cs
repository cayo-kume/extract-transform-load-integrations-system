using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;
using Domain.Core.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxCommerce
{
    public class B2CConsultaClientesRepository : IB2CConsultaClientesRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly IB2CConsultaClientesCommandHandler _commandHandler;

        public B2CConsultaClientesRepository(IIntegrationsCoreRepository coreRepository, IB2CConsultaClientesCommandHandler commandHandler)
        {
            _coreRepository = coreRepository;
            _commandHandler = commandHandler;
        }

        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<B2CConsultaClientes> records)
        {
            var table = _coreRepository.CreateSystemDataTable(jobParameter.tableName, new B2CConsultaClientes());

            _coreRepository.FillSystemDataTable(table, records.ToList());

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: table
            );

            return true;
        }

        public async Task<bool> InsertRecord(LinxAPIParam jobParameter, B2CConsultaClientes? record)
        {
            string sql = _commandHandler.CreateInsertRecordQuery(jobParameter.tableName);
            return await _coreRepository.InsertRecord(sql: sql, entity: record);
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
    }
}
