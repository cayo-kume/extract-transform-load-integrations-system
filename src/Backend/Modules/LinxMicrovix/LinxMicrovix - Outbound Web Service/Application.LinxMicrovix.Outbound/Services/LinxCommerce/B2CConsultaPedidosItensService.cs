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
    public class B2CConsultaPedidosItensService
        : B2CBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosItens, B2CConsultaPedidosItens>,
          IB2CConsultaPedidosItensService
    {
        private readonly IB2CConsultaPedidosItensRepository _b2cConsultaPedidosItensRepository;
        private readonly IB2CConsultaPedidosItensCommandHandler _b2cConsultaPedidosItensCommandHandler;

        public B2CConsultaPedidosItensService(
            IAPICall apiCall,
            ILoggerService logger,
            IIntegrationsCoreRepository coreRepository,
            ILinxMicrovixService linxMicrovixServiceBase,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosItens> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IB2CConsultaPedidosItensCommandHandler b2cConsultaPedidosItensCommandHandler,
            IB2CConsultaPedidosItensRepository b2cConsultaPedidosItensRepository,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _b2cConsultaPedidosItensRepository = b2cConsultaPedidosItensRepository;
            _b2cConsultaPedidosItensCommandHandler = b2cConsultaPedidosItensCommandHandler;
        }

        protected override EnumJob JobName => EnumJob.B2CConsultaPedidosItens;
        protected override string CacheKey => "B2CConsultaPedidosItens:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosItens CreateDto(Dictionary<string?, string?> record) =>
            new(
                id_pedido_item: record.Where(pair => pair.Key == "id_pedido_item").Select(pair => pair.Value).FirstOrDefault(),
                id_pedido: record.Where(pair => pair.Key == "id_pedido").Select(pair => pair.Value).FirstOrDefault(),
                codigoproduto: record.Where(pair => pair.Key == "codigoproduto").Select(pair => pair.Value).FirstOrDefault(),
                quantidade: record.Where(pair => pair.Key == "quantidade").Select(pair => pair.Value).FirstOrDefault(),
                vl_unitario: record.Where(pair => pair.Key == "vl_unitario").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override B2CConsultaPedidosItens CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosItens dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"id_pedido_item: {record.Where(p => p.Key == "id_pedido_item").Select(p => p.Value).FirstOrDefault()} | codigoproduto: {record.Where(p => p.Key == "codigoproduto").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(B2CConsultaPedidosItens entity) => entity.recordKey;
        protected override string? GetRecordXml(B2CConsultaPedidosItens entity) => entity.recordXml;

        protected override string BuildEntityLogContext(B2CConsultaPedidosItens entity) =>
            $"id_pedido_item: {entity.id_pedido_item} | codigoproduto: {entity.codigoproduto}";

        protected override string BuildRequestBodyParameters(string parameters, LinxAPIParam jobParameter)
        {
            string? timestamp = _linxMicrovixCommandHandler.CreateGetLast7DaysMinTimestampQuery(
                jobParameter.schema,
                jobParameter.tableName,
                columnDate: "lastupdateon"
            );

            return parameters.Replace("[0]", timestamp);
        }

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<B2CConsultaPedidosItens> records) =>
            await _b2cConsultaPedidosItensRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(y => y.id_pedido_item).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<B2CConsultaPedidosItens> records, LinxAPIParam jobParameter) =>
            _b2cConsultaPedidosItensRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<B2CConsultaPedidosItens?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<B2CConsultaPedidosItens?>().ToList();

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
                    await _b2cConsultaPedidosItensRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> IntegrityLockRegisters(LinxAPIParam jobParameter)
        {
            try
            {
                var _listSomenteNovos = new List<B2CConsultaPedidosItens>();

                string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
                string? parameters = await _coreRepository.GetRecord<string>(sql);

                string integritySql = _b2cConsultaPedidosItensCommandHandler.CreateIntegrityLockQuery();
                var _listRegistrosConsultados = await _coreRepository.GetRecords<IntegrityLockTablesRegister>(integritySql);

                foreach (var registro in _listRegistrosConsultados)
                {
                    var body = _linxMicrovixServiceBase.BuildBodyRequest(
                                    parametersList: parameters.Replace("[0]", "0").Replace("[id_pedido]", registro.identifier),
                                    jobParameter: jobParameter,
                                    cnpj_emp: jobParameter.DocMainCompany
                                );

                    string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                    var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

                    if (xmls.Count() > 0)
                    {
                        var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);
                        _listSomenteNovos.AddRange(listRecords);
                        registro.is_present_in_erp = true;
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
