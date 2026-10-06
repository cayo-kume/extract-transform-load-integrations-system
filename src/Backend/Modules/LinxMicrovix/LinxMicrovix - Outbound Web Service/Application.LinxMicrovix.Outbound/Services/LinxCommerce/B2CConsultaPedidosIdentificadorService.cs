using Application.Core.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxCommerce;
using Domain.Core.Abstractions.Repositorys;
using Domain.Core.Enums;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace Application.LinxMicrovix.Outbound.WebService.Services
{
    public class B2CConsultaPedidosIdentificadorService
        : B2CBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosIdentificador, B2CConsultaPedidosIdentificador>,
          IB2CConsultaPedidosIdentificadorService
    {
        private readonly IB2CConsultaPedidosIdentificadorRepository _b2cConsultaPedidosIdentificadorRepository;
        private readonly IB2CConsultaPedidosIdentificadorCommandHandler _b2cConsultaPedidosIdentifcadorCommandHandler;

        public B2CConsultaPedidosIdentificadorService(
            IAPICall apiCall,
            ILoggerService logger,
            IIntegrationsCoreRepository coreRepository,
            ILinxMicrovixService linxMicrovixServiceBase,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosIdentificador> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IB2CConsultaPedidosIdentificadorCommandHandler b2cConsultaPedidosIdentifcadorCommandHandler,
            IB2CConsultaPedidosIdentificadorRepository b2cConsultaPedidosIdentificadorRepository,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _b2cConsultaPedidosIdentificadorRepository = b2cConsultaPedidosIdentificadorRepository;
            _b2cConsultaPedidosIdentifcadorCommandHandler = b2cConsultaPedidosIdentifcadorCommandHandler;
        }

        protected override EnumJob JobName => EnumJob.B2CConsultaPedidosIdentificador;
        protected override string CacheKey => "B2CConsultaPedidosIdentificador:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosIdentificador CreateDto(Dictionary<string?, string?> record) =>
            new(
                empresa: record.Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                identificador: record.Where(pair => pair.Key == "identificador").Select(pair => pair.Value).FirstOrDefault(),
                id_venda: record.Where(pair => pair.Key == "id_venda").Select(pair => pair.Value).FirstOrDefault(),
                id_cliente: record.Where(pair => pair.Key == "id_cliente").Select(pair => pair.Value).FirstOrDefault(),
                valor_frete: record.Where(pair => pair.Key == "valor_frete").Select(pair => pair.Value).FirstOrDefault(),
                data_origem: record.Where(pair => pair.Key == "data_origem").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                order_id: record.Where(pair => pair.Key == "order_id").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override B2CConsultaPedidosIdentificador CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosIdentificador dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"order_id: {record.Where(p => p.Key == "order_id").Select(p => p.Value).FirstOrDefault()} | identificador: {record.Where(p => p.Key == "identificador").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(B2CConsultaPedidosIdentificador entity) => entity.recordKey;
        protected override string? GetRecordXml(B2CConsultaPedidosIdentificador entity) => entity.recordXml;

        protected override string BuildEntityLogContext(B2CConsultaPedidosIdentificador entity) =>
            $"order_id: {entity.order_id} | identificador: {entity.identificador}";

        protected override string BuildRequestBodyParameters(string parameters, LinxAPIParam jobParameter) =>
            parameters
                .Replace("[0]", "0")
                .Replace("[data_origem_inicial]", $"{DateTime.Today.AddDays(-20).ToString("yyyy-MM-dd")}")
                .Replace("[data_origem_fim]", $"{DateTime.Today.AddDays(+1).ToString("yyyy-MM-dd")}");

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<B2CConsultaPedidosIdentificador> records) =>
            await _b2cConsultaPedidosIdentificadorRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records
            );

        protected override void BulkInsertIntoTableRaw(List<B2CConsultaPedidosIdentificador> records, LinxAPIParam jobParameter) =>
            _b2cConsultaPedidosIdentificadorRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<B2CConsultaPedidosIdentificador?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<B2CConsultaPedidosIdentificador?>().ToList();

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                        parametersList: BuildRequestBodyParameters(parameters, jobParameter),
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

                var _listSomenteNovos = listRecords.Where(x => !recordKeys.Contains(GetRecordKey(x)?.ToUpper())).ToList();

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

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                        parametersList: parameters.Replace("[0]", "0").Replace("[order_id]", identificador),
                        jobParameter: jobParameter,
                        cnpj_emp: cnpj_emp
                    );

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _b2cConsultaPedidosIdentificadorRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> IntegrityLockRegisters(LinxAPIParam jobParameter)
        {
            try
            {
                var _listSomenteNovos = new List<B2CConsultaPedidosIdentificador>();

                string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
                string? parameters = await _coreRepository.GetRecord<string>(sql);

                string integritySql = _b2cConsultaPedidosIdentifcadorCommandHandler.CreateIntegrityLockQuery();
                var _listRegistrosConsultados = await _coreRepository.GetRecords<IntegrityLockTablesRegister>(integritySql);

                foreach (var pedido in _listRegistrosConsultados)
                {
                    var body = _linxMicrovixServiceBase.BuildBodyRequest(
                                    parametersList: parameters.Replace("[0]", "0").Replace("[id_pedido]", pedido.identifier),
                                    jobParameter: jobParameter,
                                    cnpj_emp: jobParameter.DocMainCompany
                                );

                    string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                    var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

                    if (xmls.Count() > 0)
                    {
                        var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);
                        _listSomenteNovos.AddRange(listRecords);
                        pedido.is_present_in_erp = true;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
