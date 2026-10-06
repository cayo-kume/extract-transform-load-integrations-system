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
    public class LinxSetoresService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxSetores, LinxSetores>,
          ILinxSetoresService
    {
        private readonly ILinxSetoresRepository _linxSetoresRepository;

        public LinxSetoresService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxSetoresRepository linxSetoresRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxSetores> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxSetoresRepository = linxSetoresRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxSetores;
        protected override string CacheKey => "LinxSetores:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxSetores CreateDto(Dictionary<string?, string?> record) =>
            new(
                id_setor: record.Where(pair => pair.Key == "id_setor").Select(pair => pair.Value).FirstOrDefault(),
                desc_setor: record.Where(pair => pair.Key == "desc_setor").Select(pair => pair.Value).FirstOrDefault(),
                codigo_integracao_ws: record.Where(pair => pair.Key == "codigo_integracao_ws").Select(pair => pair.Value).FirstOrDefault(),
                ativo: record.Where(pair => pair.Key == "ativo").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxSetores CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxSetores dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"id_setor: {record.Where(p => p.Key == "id_setor").Select(p => p.Value).FirstOrDefault()} | desc_setor: {record.Where(p => p.Key == "desc_setor").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxSetores entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxSetores entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxSetores entity) =>
            $"id_setor: {entity.id_setor} | desc_setor: {entity.desc_setor}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxSetores> records)
        {
            var list = await _linxSetoresRepository.GetRegistersExists();
            return list.Cast<string?>();
        }

        protected override void BulkInsertIntoTableRaw(List<LinxSetores> records, LinxAPIParam jobParameter) =>
            _linxSetoresRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxSetores?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxSetores?>().ToList();

        public Task<bool> GetRecords(LinxAPIParam jobParameter) => GetRecordsCore(jobParameter);

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[id_setor]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxSetoresRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }
    }
}
