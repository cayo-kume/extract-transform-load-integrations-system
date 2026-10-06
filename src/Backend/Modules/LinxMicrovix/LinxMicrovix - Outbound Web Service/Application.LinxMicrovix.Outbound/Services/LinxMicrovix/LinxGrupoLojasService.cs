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
    public class LinxGrupoLojasService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxGrupoLojas, LinxGrupoLojas>,
          ILinxGrupoLojasService
    {
        private readonly ILinxGrupoLojasRepository _linxGrupoLojasRepository;

        public LinxGrupoLojasService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxGrupoLojasRepository linxGrupoLojasRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxGrupoLojas> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxGrupoLojasRepository = linxGrupoLojasRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxGrupoLojas;
        protected override string CacheKey => "LinxGrupoLojas:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxGrupoLojas CreateDto(Dictionary<string?, string?> record) =>
            new(
                cnpj: record.Where(pair => pair.Key == "CNPJ").Select(pair => pair.Value).FirstOrDefault(),
                nome_empresa: record.Where(pair => pair.Key == "nome_empresa").Select(pair => pair.Value).FirstOrDefault(),
                id_empresas_rede: record.Where(pair => pair.Key == "id_empresas_rede").Select(pair => pair.Value).FirstOrDefault(),
                rede: record.Where(pair => pair.Key == "rede").Select(pair => pair.Value).FirstOrDefault(),
                nome_portal: record.Where(pair => pair.Key == "nome_portal").Select(pair => pair.Value).FirstOrDefault(),
                empresa: record.Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                classificacao_portal: record.Where(pair => pair.Key == "classificacao_portal").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxGrupoLojas CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxGrupoLojas dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"cnpj: {record.Where(p => p.Key == "cnpj").Select(p => p.Value).FirstOrDefault()} | nome_empresa: {record.Where(p => p.Key == "nome_empresa").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxGrupoLojas entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxGrupoLojas entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxGrupoLojas entity) =>
            $"cnpj: {entity.cnpj} | nome_empresa: {entity.nome_empresa}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxGrupoLojas> records) =>
            await _linxGrupoLojasRepository.GetRegistersExists();

        protected override void BulkInsertIntoTableRaw(List<LinxGrupoLojas> records, LinxAPIParam jobParameter) =>
            _linxGrupoLojasRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxGrupoLojas?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxGrupoLojas?>().ToList();

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            var sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            var result = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                        jobParameter: jobParameter
                    );

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

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
