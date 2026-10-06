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
    public class LinxClientesEnderecosEntregaService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxClientesEnderecosEntrega, LinxClientesEnderecosEntrega>,
          ILinxClientesEnderecosEntregaService
    {
        private readonly ILinxClientesEnderecosEntregaRepository _linxClientesEnderecosEntregaRepository;

        public LinxClientesEnderecosEntregaService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxClientesEnderecosEntregaRepository linxClientesEnderecosEntregaRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxClientesEnderecosEntrega> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxClientesEnderecosEntregaRepository = linxClientesEnderecosEntregaRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxClientesEnderecosEntrega;
        protected override string CacheKey => "LinxClientesEnderecosEntrega:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxClientesEnderecosEntrega CreateDto(Dictionary<string?, string?> record) =>
            new(
                id_endereco_entrega: record.Where(pair => pair.Key == "id_endereco_entrega").Select(pair => pair.Value).FirstOrDefault(),
                cod_cliente: record.Where(pair => pair.Key == "cod_cliente").Select(pair => pair.Value).FirstOrDefault(),
                endereco_cliente: record.Where(pair => pair.Key == "endereco_cliente").Select(pair => pair.Value).FirstOrDefault(),
                numero_rua_cliente: record.Where(pair => pair.Key == "numero_rua_cliente").Select(pair => pair.Value).FirstOrDefault(),
                complemento_end_cli: record.Where(pair => pair.Key == "complement_end_cli").Select(pair => pair.Value).FirstOrDefault(),
                bairro_cliente: record.Where(pair => pair.Key == "bairro_cliente").Select(pair => pair.Value).FirstOrDefault(),
                cep_cliente: record.Where(pair => pair.Key == "cep_cliente").Select(pair => pair.Value).FirstOrDefault(),
                cidade_cliente: record.Where(pair => pair.Key == "cidade_cliente").Select(pair => pair.Value).FirstOrDefault(),
                uf_cliente: record.Where(pair => pair.Key == "uf_cliente").Select(pair => pair.Value).FirstOrDefault(),
                descricao: record.Where(pair => pair.Key == "descricao").Select(pair => pair.Value).FirstOrDefault(),
                principal: record.Where(pair => pair.Key == "principal").Select(pair => pair.Value).FirstOrDefault(),
                fone_cliente: record.Where(pair => pair.Key == "fone_cliente").Select(pair => pair.Value).FirstOrDefault(),
                fone_celular: record.Where(pair => pair.Key == "fone_celular").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxClientesEnderecosEntrega CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxClientesEnderecosEntrega dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"id_endereco_entrega: {record.Where(p => p.Key == "id_endereco_entrega").Select(p => p.Value).FirstOrDefault()} | cod_cliente: {record.Where(p => p.Key == "cod_cliente").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxClientesEnderecosEntrega entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxClientesEnderecosEntrega entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxClientesEnderecosEntrega entity) =>
            $"id_endereco_entrega: {entity.id_endereco_entrega} | cod_cliente: {entity.cod_cliente}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxClientesEnderecosEntrega> records) =>
            await _linxClientesEnderecosEntregaRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.id_endereco_entrega).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxClientesEnderecosEntrega> records, LinxAPIParam jobParameter) =>
            _linxClientesEnderecosEntregaRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxClientesEnderecosEntrega?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxClientesEnderecosEntrega?>().ToList();

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            string sqlTimestamp = _linxMicrovixCommandHandler.CreateGetLast7DaysMinTimestampQuery(jobParameter.schema, jobParameter.tableName, "lastupdateon");
            string? timestamp = await _coreRepository.GetRecord<string>(sqlTimestamp);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                        parametersList: parameters.Replace("[0]", timestamp),
                        jobParameter: jobParameter,
                        cnpj_emp: jobParameter.DocMainCompany
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
