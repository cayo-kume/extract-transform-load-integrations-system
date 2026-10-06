using Application.Core.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxMicrovix;
using Domain.Core.Abstractions.Repositorys;
using Domain.Core.Enums;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Base;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace Application.LinxMicrovix.Outbound.WebService.Services.LinxMicrovix
{
    public class LinxMovimentoPlanosService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoPlanos, LinxMovimentoPlanos>,
          ILinxMovimentoPlanosService
    {
        private readonly ILinxMovimentoPlanosRepository _linxMovimentoPlanosRepository;

        public LinxMovimentoPlanosService(
            IAPICall apiCall,
            ILoggerService logger,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoPlanos> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxMovimentoPlanosRepository linxMovimentoPlanosRepository,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxMovimentoPlanosRepository = linxMovimentoPlanosRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxMovimentoPlanos;
        protected override string CacheKey => "LinxMovimentoPlanos:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoPlanos CreateDto(Dictionary<string?, string?> record) =>
            new(
                cnpj_emp: record.Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                identificador: record.Where(pair => pair.Key == "identificador").Select(pair => pair.Value).FirstOrDefault(),
                plano: record.Where(pair => pair.Key == "plano").Select(pair => pair.Value).FirstOrDefault(),
                desc_plano: record.Where(pair => pair.Key == "desc_plano").Select(pair => pair.Value).FirstOrDefault(),
                total: record.Where(pair => pair.Key == "total").Select(pair => pair.Value).FirstOrDefault(),
                qtde_parcelas: record.Where(pair => pair.Key == "qtde_parcelas").Select(pair => pair.Value).FirstOrDefault(),
                indice_plano: record.Where(pair => pair.Key == "indice_plano").Select(pair => pair.Value).FirstOrDefault(),
                cod_forma_pgto: record.Where(pair => pair.Key == "cod_forma_pgto").Select(pair => pair.Value).FirstOrDefault(),
                forma_pgto: record.Where(pair => pair.Key == "forma_pgto").Select(pair => pair.Value).FirstOrDefault(),
                tipo_transacao: record.Where(pair => pair.Key == "tipo_transacao").Select(pair => pair.Value).FirstOrDefault(),
                taxa_financeira: record.Where(pair => pair.Key == "taxa_financeira").Select(pair => pair.Value).FirstOrDefault(),
                ordem_cartao: record.Where(pair => pair.Key == "ordem_cartao").Select(pair => pair.Value).FirstOrDefault(),
                empresa: record.Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxMovimentoPlanos CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoPlanos dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"identificador: {record.Where(p => p.Key == "identificador").Select(p => p.Value).FirstOrDefault()} | desc_plano: {record.Where(p => p.Key == "desc_plano").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxMovimentoPlanos entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxMovimentoPlanos entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxMovimentoPlanos entity) =>
            $"identificador: {entity.identificador} | desc_plano: {entity.desc_plano}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxMovimentoPlanos> records) =>
            await _linxMovimentoPlanosRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records
            );

        protected override void BulkInsertIntoTableRaw(List<LinxMovimentoPlanos> records, LinxAPIParam jobParameter) =>
            _linxMovimentoPlanosRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxMovimentoPlanos?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxMovimentoPlanos?>().ToList();

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[identificador]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: cnpj_emp);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxMovimentoPlanosRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            var xmls = new List<Dictionary<string?, string?>>();

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            string sqlCnpjsEmp = _linxMicrovixCommandHandler.CreateGetB2CCompanysQuery();
            var cnpjs_emp = await _coreRepository.GetRecords<Company?>(sql);

            foreach (var cnpj_emp in cnpjs_emp)
            {
                string? timestamp = _linxMicrovixCommandHandler.CreateGetLast7DaysMinTimestampWithCompanyQuery(
                    jobParameter.schema,
                    jobParameter.tableName,
                    columnDate: "lastupdateon",
                    columnCompany: "cnpj_emp",
                    companyValue: cnpj_emp.Doc
                );

                var body = _linxMicrovixServiceBase.BuildBodyRequest(
                    parametersList: parameters.Replace("[0]", timestamp).Replace("[data_inicial]", $"{DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd")}").Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
                    jobParameter: jobParameter,
                    cnpj_emp: cnpj_emp.Doc
                );

                string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                var result = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);
                xmls.AddRange(result);
            }

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                var recordKeys = await _cache.GetOrCreateAsync(CacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await GetRegistersExists(jobParameter, listRecords);
                    return new HashSet<string?>(list);
                });

                var _listSomenteNovos = listRecords.Where(x => !recordKeys.Contains(GetRecordKey(x))).ToList();

                if (_listSomenteNovos.Count() > 0)
                {
                    BulkInsertIntoTableRaw(_listSomenteNovos, jobParameter);
                    await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());

                    foreach (var item in _listSomenteNovos)
                        recordKeys.Add(GetRecordKey(item));

                    for (int i = 0; i < _listSomenteNovos.Count; i++)
                    {
                        _logger.AddRecord(GetRecordKey(_listSomenteNovos[i]), GetRecordXml(_listSomenteNovos[i]));
                        _logger.AddMessage(_errorMessagesList.Where(x => x != null && x.Contains(BuildEntityLogContext(_listSomenteNovos[i]))).FirstOrDefault());
                    }
                }
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
    }
}
