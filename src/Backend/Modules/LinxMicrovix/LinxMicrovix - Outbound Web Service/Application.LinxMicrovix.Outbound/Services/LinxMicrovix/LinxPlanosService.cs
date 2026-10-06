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
    public class LinxPlanosService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPlanos, LinxPlanos>,
          ILinxPlanosService
    {
        private readonly ILinxPlanosRepository _linxPlanosRepository;

        public LinxPlanosService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            ILinxPlanosRepository linxPlanosRepository,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPlanos> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxPlanosRepository = linxPlanosRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxPlanos;
        protected override string CacheKey => "LinxPlanos:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPlanos CreateDto(Dictionary<string?, string?> record) =>
            new(
                plano: record.Where(pair => pair.Key == "plano").Select(pair => pair.Value).FirstOrDefault(),
                desc_plano: record.Where(pair => pair.Key == "desc_plano").Select(pair => pair.Value).FirstOrDefault(),
                qtde_parcelas: record.Where(pair => pair.Key == "qtde_parcelas").Select(pair => pair.Value).FirstOrDefault(),
                prazo_entre_parcelas: record.Where(pair => pair.Key == "prazo_entre_parcelas").Select(pair => pair.Value).FirstOrDefault(),
                tipo_plano: record.Where(pair => pair.Key == "tipo_plano").Select(pair => pair.Value).FirstOrDefault(),
                indice_plano: record.Where(pair => pair.Key == "indice_plano").Select(pair => pair.Value).FirstOrDefault(),
                cod_forma_pgto: record.Where(pair => pair.Key == "cod_forma_pgto").Select(pair => pair.Value).FirstOrDefault(),
                forma_pgto: record.Where(pair => pair.Key == "forma_pgto").Select(pair => pair.Value).FirstOrDefault(),
                conta_central: record.Where(pair => pair.Key == "conta_central").Select(pair => pair.Value).FirstOrDefault(),
                tipo_transacao: record.Where(pair => pair.Key == "tipo_transacao").Select(pair => pair.Value).FirstOrDefault(),
                taxa_financeira: record.Where(pair => pair.Key == "taxa_financeira").Select(pair => pair.Value).FirstOrDefault(),
                dt_upd: record.Where(pair => pair.Key == "dt_upd").Select(pair => pair.Value).FirstOrDefault(),
                desativado: record.Where(pair => pair.Key == "desativado").Select(pair => pair.Value).FirstOrDefault(),
                usa_tef: record.Where(pair => pair.Key == "usa_tef").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxPlanos CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPlanos dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"plano: {record.Where(p => p.Key == "plano").Select(p => p.Value).FirstOrDefault()} | desc_plano: {record.Where(p => p.Key == "desc_plano").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxPlanos entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxPlanos entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxPlanos entity) =>
            $"plano: {entity.plano} | desc_plano: {entity.desc_plano}";

        protected override string BuildRequestBodyParameters(string parameters, LinxAPIParam jobParameter) =>
            parameters
                .Replace("[0]", "0")
                .Replace("[data_upd_inicial]", $"2000-01-01")
                .Replace("[data_upd_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}");

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxPlanos> records) =>
            await _linxPlanosRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.plano).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxPlanos> records, LinxAPIParam jobParameter) =>
            _linxPlanosRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxPlanos?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxPlanos?>().ToList();

        public Task<bool> GetRecords(LinxAPIParam jobParameter) => GetRecordsCore(jobParameter);

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters
                                .Replace("[0]", "0")
                                .Replace("[data_upd_inicial]", $"2000-01-01")
                                .Replace("[data_upd_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}")
                                .Replace("[plano]", identificador
                ),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxPlanosRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }
    }
}
