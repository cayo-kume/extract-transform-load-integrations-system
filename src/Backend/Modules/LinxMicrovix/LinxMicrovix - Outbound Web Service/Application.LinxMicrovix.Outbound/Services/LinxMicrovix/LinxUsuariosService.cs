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
    public class LinxUsuariosService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxUsuarios, LinxUsuarios>,
          ILinxUsuariosService
    {
        private readonly ILinxUsuariosRepository _linxUsuariosRepository;

        public LinxUsuariosService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxUsuariosRepository linxUsuariosRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxUsuarios> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxUsuariosRepository = linxUsuariosRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxUsuarios;
        protected override string CacheKey => "LinxUsuarios:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxUsuarios CreateDto(Dictionary<string?, string?> record) =>
            new(
                usuario_id: record.Where(pair => pair.Key == "usuario_id").Select(pair => pair.Value).FirstOrDefault(),
                usuario_login: record.Where(pair => pair.Key == "usuario_login").Select(pair => pair.Value).FirstOrDefault(),
                usuario_nome: record.Where(pair => pair.Key == "usuario_nome").Select(pair => pair.Value).FirstOrDefault(),
                usuario_email: record.Where(pair => pair.Key == "usuario_email").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault(),
                usuario_grupo_id: record.Where(pair => pair.Key == "usuario_grupo_id").Select(pair => pair.Value).FirstOrDefault(),
                grupo_usuarios: record.Where(pair => pair.Key == "grupo_usuarios").Select(pair => pair.Value).FirstOrDefault(),
                usuario_supervisor: record.Where(pair => pair.Key == "usuario_supervisor").Select(pair => pair.Value).FirstOrDefault(),
                usuario_doc: record.Where(pair => pair.Key == "usuario_doc").Select(pair => pair.Value).FirstOrDefault(),
                vendedor: record.Where(pair => pair.Key == "vendedor").Select(pair => pair.Value).FirstOrDefault(),
                data_criacao: record.Where(pair => pair.Key == "data_criacao").Select(pair => pair.Value).FirstOrDefault(),
                desativado: record.Where(pair => pair.Key == "desativado").Select(pair => pair.Value).FirstOrDefault(),
                empresas: record.Where(pair => pair.Key == "empresas").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxUsuarios CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxUsuarios dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"usuario_id: {record.Where(p => p.Key == "usuario_id").Select(p => p.Value).FirstOrDefault()} | usuario_nome: {record.Where(p => p.Key == "usuario_nome").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxUsuarios entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxUsuarios entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxUsuarios entity) =>
            $"usuario_id: {entity.usuario_id} | usuario_nome: {entity.usuario_nome}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxUsuarios> records) =>
            await _linxUsuariosRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.usuario_id).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxUsuarios> records, LinxAPIParam jobParameter) =>
            _linxUsuariosRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxUsuarios?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxUsuarios?>().ToList();

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            // LinxUsuarios intentionally uses the 2-arg BuildBodyRequest overload (without cnpjEmp).
            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                    parametersList: parameters.Replace("[0]", "0"),
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
