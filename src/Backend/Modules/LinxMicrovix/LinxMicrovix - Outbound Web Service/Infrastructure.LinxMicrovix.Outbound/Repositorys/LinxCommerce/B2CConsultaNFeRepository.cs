using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;
using Domain.Core.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxCommerce
{
    public class B2CConsultaNFeRepository : IB2CConsultaNFeRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly IB2CConsultaNFeCommandHandler _commandHandler;

        public B2CConsultaNFeRepository(IIntegrationsCoreRepository coreRepository, IB2CConsultaNFeCommandHandler commandHandler)
        {
            _coreRepository = coreRepository;
            _commandHandler = commandHandler;
        }

        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<B2CConsultaNFe> records)
        {
            var table = _coreRepository.CreateSystemDataTable(jobParameter.tableName, new B2CConsultaNFe());

            _coreRepository.FillSystemDataTable(table, records.ToList());

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: table
            );

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

        public async Task<bool> InsertRecord(LinxAPIParam jobParameter, B2CConsultaNFe? record)
        {
            string sql = _commandHandler.CreateInsertRecordQuery(jobParameter.tableName);
            return await _coreRepository.InsertRecord(sql: sql, entity: record);
        }
    }
}