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
    public class LinxCfopFiscalService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxCfopFiscal, LinxCfopFiscal>,
          ILinxCfopFiscalService
    {
        private readonly ILinxCfopFiscalRepository _linxCfopFiscalRepository;

        public LinxCfopFiscalService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxCfopFiscalRepository linxCfopFiscalRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxCfopFiscal> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxCfopFiscalRepository = linxCfopFiscalRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxCfopFiscal;
        protected override string CacheKey => "LinxCfopFiscal:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxCfopFiscal CreateDto(Dictionary<string?, string?> record) =>
            new(
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault(),
                id_cfop_fiscal: record.Where(pair => pair.Key == "id_cfop_fiscal").Select(pair => pair.Value).FirstOrDefault(),
                cfop_fiscal: record.Where(pair => pair.Key == "cfop_fiscal").Select(pair => pair.Value).FirstOrDefault(),
                descricao: record.Where(pair => pair.Key == "descricao").Select(pair => pair.Value).FirstOrDefault(),
                excluido: record.Where(pair => pair.Key == "excluido").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxCfopFiscal CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxCfopFiscal dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"id_cfop_fiscal: {record.Where(p => p.Key == "id_cfop_fiscal").Select(p => p.Value).FirstOrDefault()} | descricao: {record.Where(p => p.Key == "descricao").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxCfopFiscal entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxCfopFiscal entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxCfopFiscal entity) =>
            $"id_cfop_fiscal: {entity.id_cfop_fiscal} | descricao: {entity.descricao}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxCfopFiscal> records) =>
            await _linxCfopFiscalRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.id_cfop_fiscal).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxCfopFiscal> records, LinxAPIParam jobParameter) =>
            _linxCfopFiscalRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxCfopFiscal?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxCfopFiscal?>().ToList();

        public Task<bool> GetRecords(LinxAPIParam jobParameter) => GetRecordsCore(jobParameter);

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[cfop_fiscal]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxCfopFiscalRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }
    }
}
