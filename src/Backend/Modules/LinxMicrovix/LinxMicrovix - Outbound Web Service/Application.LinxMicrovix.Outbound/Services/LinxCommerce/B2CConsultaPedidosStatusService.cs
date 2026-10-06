using Application.Core.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
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
    public class B2CConsultaPedidosStatusService
        : B2CBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosStatus, B2CConsultaPedidosStatus>,
          IB2CConsultaPedidosStatusService
    {
        private readonly IB2CConsultaPedidosStatusRepository _b2cConsultaPedidosStatusRepository;
        //private readonly IB2CConsultaPedidosStatusCommandHandler _b2ConsultaPedidosStatusCommandHandler;

        public B2CConsultaPedidosStatusService(
            IAPICall apiCall,
            ILoggerService logger,
            IIntegrationsCoreRepository coreRepository,
            ILinxMicrovixService linxMicrovixServiceBase,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosStatus> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            //IB2CConsultaPedidosStatusCommandHandler b2ConsultaPedidosStatusCommandHandler,
            IB2CConsultaPedidosStatusRepository b2cConsultaPedidosStatusRepository,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _b2cConsultaPedidosStatusRepository = b2cConsultaPedidosStatusRepository;
            //_b2ConsultaPedidosStatusCommandHandler = b2ConsultaPedidosStatusCommandHandler;
        }

        protected override EnumJob JobName => EnumJob.B2CConsultaPedidosStatus;
        protected override string CacheKey => "B2CConsultaPedidosStatus:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosStatus CreateDto(Dictionary<string?, string?> record) =>
            new(
                id: record.Where(pair => pair.Key == "id").Select(pair => pair.Value).FirstOrDefault(),
                id_status: record.Where(pair => pair.Key == "id_status").Select(pair => pair.Value).FirstOrDefault(),
                id_pedido: record.Where(pair => pair.Key == "id_pedido").Select(pair => pair.Value).FirstOrDefault(),
                data_hora: record.Where(pair => pair.Key == "data_hora").Select(pair => pair.Value).FirstOrDefault(),
                anotacao: record.Where(pair => pair.Key == "anotacao").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override B2CConsultaPedidosStatus CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosStatus dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"id_pedido: {record.Where(p => p.Key == "id_pedido").Select(p => p.Value).FirstOrDefault()} | id_status: {record.Where(p => p.Key == "id_status").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(B2CConsultaPedidosStatus entity) => entity.recordKey;
        protected override string? GetRecordXml(B2CConsultaPedidosStatus entity) => entity.recordXml;

        protected override string BuildEntityLogContext(B2CConsultaPedidosStatus entity) =>
            $"id_pedido: {entity.id_pedido} | id_status: {entity.id_status}";

        protected override string BuildRequestBodyParameters(string parameters, LinxAPIParam jobParameter) =>
            parameters
                .Replace("[0]", "0")
                .Replace("[data_inicial]", $"{DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}")
                .Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}");

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<B2CConsultaPedidosStatus> records) =>
            await _b2cConsultaPedidosStatusRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.id).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<B2CConsultaPedidosStatus> records, LinxAPIParam jobParameter) =>
            _b2cConsultaPedidosStatusRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<B2CConsultaPedidosStatus?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<B2CConsultaPedidosStatus?>().ToList();

        public Task<bool> GetRecords(LinxAPIParam jobParameter) => GetRecordsCore(jobParameter);

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[id_pedido]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _b2cConsultaPedidosStatusRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> IntegrityLockRegisters(LinxAPIParam jobParameter)
        {
            try
            {
                var _listSomenteNovos = new List<B2CConsultaPedidosStatus>();

                string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
                string? parameters = await _coreRepository.GetRecord<string>(sql);

                string integritySql = "";// _b2ConsultaPedidosStatusCommandHandler.CreateIntegrityLockQuery();
                var _listRegistrosConsultados = await _coreRepository.GetRecords<IntegrityLockTablesRegister>(integritySql);

                foreach (var pedidoStatus in _listRegistrosConsultados)
                {
                    var body = _linxMicrovixServiceBase.BuildBodyRequest(
                                    parametersList: parameters.Replace("[0]", "0").Replace("[id_pedido]", pedidoStatus.identifier).Replace("[data_inicial]", "2000-01-01").Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
                                    jobParameter: jobParameter,
                                    cnpj_emp: jobParameter.DocMainCompany
                                );

                    string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                    var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

                    if (xmls.Count() > 0)
                    {
                        var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);
                        _listSomenteNovos.AddRange(listRecords);
                        pedidoStatus.is_present_in_erp = true;
                    }
                }

                if (_listRegistrosConsultados.Count() > 0)
                {
                    _b2cConsultaPedidosStatusRepository.BulkInsertIntoTableRaw(records: _listSomenteNovos, jobParameter: jobParameter);
                    await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
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
