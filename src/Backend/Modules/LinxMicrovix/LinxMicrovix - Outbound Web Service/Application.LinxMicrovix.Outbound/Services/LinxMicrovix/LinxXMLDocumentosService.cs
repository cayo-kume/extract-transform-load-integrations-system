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
    public class LinxXMLDocumentosService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxXMLDocumentos, LinxXMLDocumentos>,
          ILinxXMLDocumentosService
    {
        private readonly ILinxXMLDocumentosRepository _linxXMLDocumentosRepository;

        public LinxXMLDocumentosService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            ILinxXMLDocumentosRepository linxXMLDocumentosRepository,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxXMLDocumentos> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxXMLDocumentosRepository = linxXMLDocumentosRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxXMLDocumentos;
        protected override string CacheKey => "LinxXMLDocumentos:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxXMLDocumentos CreateDto(Dictionary<string?, string?> record) =>
            new(
                cnpj_emp: record.Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                documento: record.Where(pair => pair.Key == "documento").Select(pair => pair.Value).FirstOrDefault(),
                serie: record.Where(pair => pair.Key == "serie").Select(pair => pair.Value).FirstOrDefault(),
                data_emissao: record.Where(pair => pair.Key == "data_emissao").Select(pair => pair.Value).FirstOrDefault(),
                chave_nfe: record.Where(pair => pair.Key == "chave_nfe").Select(pair => pair.Value).FirstOrDefault(),
                situacao: record.Where(pair => pair.Key == "situacao").Select(pair => pair.Value).FirstOrDefault(),
                xml: record.Where(pair => pair.Key == "xml").Select(pair => pair.Value).FirstOrDefault(),
                excluido: record.Where(pair => pair.Key == "excluido").Select(pair => pair.Value).FirstOrDefault(),
                identificador_microvix: record.Where(pair => pair.Key == "identificador_microvix").Select(pair => pair.Value).FirstOrDefault(),
                dt_insert: record.Where(pair => pair.Key == "dt_insert").Select(pair => pair.Value).FirstOrDefault(),
                nProtCanc: record.Where(pair => pair.Key == "nProtCanc").Select(pair => pair.Value).FirstOrDefault(),
                nProtDeneg: record.Where(pair => pair.Key == "nProtDeneg").Select(pair => pair.Value).FirstOrDefault(),
                nProtInut: record.Where(pair => pair.Key == "nProtInut").Select(pair => pair.Value).FirstOrDefault(),
                xmlDistribuicao: record.Where(pair => pair.Key == "xmlDistribuicao").Select(pair => pair.Value).FirstOrDefault(),
                cStat: record.Where(pair => pair.Key == "cStat").Select(pair => pair.Value).FirstOrDefault(),
                id_nfe: record.Where(pair => pair.Key == "id_nfe").Select(pair => pair.Value).FirstOrDefault(),
                cod_cliente: record.Where(pair => pair.Key == "cod_cliente").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxXMLDocumentos CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxXMLDocumentos dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"documento: {record.Where(p => p.Key == "documento").Select(p => p.Value).FirstOrDefault()} | chave_nfe: {record.Where(p => p.Key == "chave_nfe").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxXMLDocumentos entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxXMLDocumentos entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxXMLDocumentos entity) =>
            $"documento: {entity.documento} | chave_nfe: {entity.chave_nfe}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxXMLDocumentos> records)
        {
            var list = await _linxXMLDocumentosRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.chave_nfe).ToList()
            );
            return list.Cast<string?>();
        }

        protected override void BulkInsertIntoTableRaw(List<LinxXMLDocumentos> records, LinxAPIParam jobParameter) =>
            _linxXMLDocumentosRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxXMLDocumentos?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxXMLDocumentos?>().ToList();

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[documento]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: cnpj_emp);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxXMLDocumentosRepository.InsertRecord(record: record, jobParameter: jobParameter);
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
                var body = _linxMicrovixServiceBase.BuildBodyRequest(
                            parametersList: parameters
                                .Replace("[0]", "0")
                                .Replace("[data_inicial]", $"{DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}")
                                .Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
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
