using Application.Core.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxCommerce;
using Domain.Core.Abstractions.Repositorys;
using Domain.Core.Enums;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Base;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace Application.LinxMicrovix.Outbound.WebService.Services
{
    public class B2CConsultaStatusService
        : B2CBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaStatus, B2CConsultaStatus>,
          IB2CConsultaStatusService
    {
        private readonly IB2CConsultaStatusRepository _b2cConsultaStatusRepository;

        public B2CConsultaStatusService(
            IAPICall apiCall,
            ILoggerService logger,
            IIntegrationsCoreRepository coreRepository,
            ILinxMicrovixService linxMicrovixServiceBase,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaStatus> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IB2CConsultaStatusRepository b2cConsultaStatusRepository,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _b2cConsultaStatusRepository = b2cConsultaStatusRepository;
        }

        protected override EnumJob JobName => EnumJob.B2CConsultaStatus;
        protected override string CacheKey => "B2CConsultaStatus:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaStatus CreateDto(Dictionary<string?, string?> record) =>
            new(
                id_status: record.Where(pair => pair.Key == "id_status").Select(pair => pair.Value).FirstOrDefault(),
                descricao_status: record.Where(pair => pair.Key == "descricao_status").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override B2CConsultaStatus CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaStatus dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"id_status: {record.Where(p => p.Key == "id_status").Select(p => p.Value).FirstOrDefault()} | descricao_status: {record.Where(p => p.Key == "descricao_status").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(B2CConsultaStatus entity) => entity.recordKey;
        protected override string? GetRecordXml(B2CConsultaStatus entity) => entity.recordXml;

        protected override string BuildEntityLogContext(B2CConsultaStatus entity) =>
            $"id_status: {entity.id_status} | descricao_status: {entity.descricao_status}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<B2CConsultaStatus> records) =>
            await _b2cConsultaStatusRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.id_status).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<B2CConsultaStatus> records, LinxAPIParam jobParameter) =>
            _b2cConsultaStatusRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<B2CConsultaStatus?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<B2CConsultaStatus?>().ToList();

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
                var body = _linxMicrovixServiceBase.BuildBodyRequest(
                            parametersList: parameters.Replace("[0]", "0"),
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
