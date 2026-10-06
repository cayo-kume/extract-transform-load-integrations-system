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
    public class LinxMovimentoTrocasService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoTrocas, LinxMovimentoTrocas>,
          ILinxMovimentoTrocasService
    {
        private readonly ILinxMovimentoTrocasRepository _linxMovimentoTrocasRepository;

        public LinxMovimentoTrocasService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            ILinxMovimentoTrocasRepository linxMovimentoTrocasRepository,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoTrocas> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxMovimentoTrocasRepository = linxMovimentoTrocasRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxMovimentoTrocas;
        protected override string CacheKey => "LinxMovimentoTrocas:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoTrocas CreateDto(Dictionary<string?, string?> record) =>
            new(
                cnpj_emp: record.Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                identificador: record.Where(pair => pair.Key == "identificador").Select(pair => pair.Value).FirstOrDefault(),
                num_vale: record.Where(pair => pair.Key == "num_vale").Select(pair => pair.Value).FirstOrDefault(),
                valor_vale: record.Where(pair => pair.Key == "valor_vale").Select(pair => pair.Value).FirstOrDefault(),
                motivo: record.Where(pair => pair.Key == "motivo").Select(pair => pair.Value).FirstOrDefault(),
                doc_origem: record.Where(pair => pair.Key == "doc_origem").Select(pair => pair.Value).FirstOrDefault(),
                serie_origem: record.Where(pair => pair.Key == "serie_origem").Select(pair => pair.Value).FirstOrDefault(),
                doc_venda: record.Where(pair => pair.Key == "doc_venda").Select(pair => pair.Value).FirstOrDefault(),
                serie_venda: record.Where(pair => pair.Key == "serie_venda").Select(pair => pair.Value).FirstOrDefault(),
                excluido: record.Where(pair => pair.Key == "excluido").Select(pair => pair.Value).FirstOrDefault(),
                desfazimento: record.Where(pair => pair.Key == "desfazimento").Select(pair => pair.Value).FirstOrDefault(),
                empresa: record.Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                vale_cod_cliente: record.Where(pair => pair.Key == "vale_cod_cliente").Select(pair => pair.Value).FirstOrDefault(),
                vale_codigoproduto: record.Where(pair => pair.Key == "vale_codigoproduto").Select(pair => pair.Value).FirstOrDefault(),
                id_vale_ordem_servico_externa: record.Where(pair => pair.Key == "id_vale_ordem_servico_externa").Select(pair => pair.Value).FirstOrDefault(),
                doc_venda_origem: record.Where(pair => pair.Key == "doc_venda_origem").Select(pair => pair.Value).FirstOrDefault(),
                serie_venda_origem: record.Where(pair => pair.Key == "serie_venda_origem").Select(pair => pair.Value).FirstOrDefault(),
                cod_cliente: record.Where(pair => pair.Key == "cod_cliente").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxMovimentoTrocas CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoTrocas dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"identificador: {record.Where(p => p.Key == "identificador").Select(p => p.Value).FirstOrDefault()} | num_vale: {record.Where(p => p.Key == "num_vale").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxMovimentoTrocas entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxMovimentoTrocas entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxMovimentoTrocas entity) =>
            $"identificador: {entity.identificador} | num_vale: {entity.num_vale}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxMovimentoTrocas> records) =>
            await _linxMovimentoTrocasRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records
                            .GroupBy(x => x.num_vale)
                            .Select(y => y.First())
                            .Cast<LinxMovimentoTrocas?>()
                            .ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxMovimentoTrocas> records, LinxAPIParam jobParameter) =>
            _linxMovimentoTrocasRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxMovimentoTrocas?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxMovimentoTrocas?>().ToList();

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[doc_origem]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: cnpj_emp);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxMovimentoTrocasRepository.InsertRecord(record: record, jobParameter: jobParameter);
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

            var sqlCnpjs = _linxMicrovixCommandHandler.CreateGetMicrovixCompanysQuery();
            var cnpjs_emp = await _coreRepository.GetRecords<Company>(sqlCnpjs);

            foreach (var cnpj_emp in cnpjs_emp)
            {
                string? sqlTimestamp = _linxMicrovixCommandHandler.CreateGetLast7DaysMinTimestampWithCompanyQuery(
                    jobParameter.schema,
                    jobParameter.tableName,
                    columnDate: "lastupdateon",
                    columnCompany: "cnpj_emp",
                    companyValue: cnpj_emp.Doc
                );

                var timestamp = await _coreRepository.GetRecord<string>(sqlTimestamp);

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
