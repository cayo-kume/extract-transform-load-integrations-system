using Application.Core.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxMicrovix;
using Domain.Core.Abstractions.Repositorys;
using Domain.Core.Enums;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace Application.LinxMicrovix.Outbound.WebService.Services.LinxMicrovix
{
    public class LinxRotinaOrigemService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxRotinaOrigem, LinxRotinaOrigem>,
          ILinxRotinaOrigemService
    {
        private readonly ILinxRotinaOrigemRepository _linxRotinaOrigemRepository;

        public LinxRotinaOrigemService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            ILinxRotinaOrigemRepository linxRotinaOrigemRepository,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxRotinaOrigem> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxRotinaOrigemRepository = linxRotinaOrigemRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxRotinaOrigem;
        protected override string CacheKey => "LinxRotinaOrigem:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxRotinaOrigem CreateDto(Dictionary<string?, string?> record) =>
            new(
                codigo_rotina: record.Where(pair => pair.Key == "codigo_rotina").Select(pair => pair.Value).FirstOrDefault(),
                descricao_rotina: record.Where(pair => pair.Key == "descricao_rotina").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxRotinaOrigem CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxRotinaOrigem dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"codigo_rotina: {record.Where(p => p.Key == "codigo_rotina").Select(p => p.Value).FirstOrDefault()} | descricao_rotina: {record.Where(p => p.Key == "descricao_rotina").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxRotinaOrigem entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxRotinaOrigem entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxRotinaOrigem entity) =>
            $"codigo_rotina: {entity.codigo_rotina} | descricao_rotina: {entity.descricao_rotina}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxRotinaOrigem> records) =>
            await _linxRotinaOrigemRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.codigo_rotina).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxRotinaOrigem> records, LinxAPIParam jobParameter) =>
            _linxRotinaOrigemRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxRotinaOrigem?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxRotinaOrigem?>().ToList();

        public Task<bool> GetRecords(LinxAPIParam jobParameter) => GetRecordsCore(jobParameter);

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[codigo_rotina]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxRotinaOrigemRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }
    }
}
