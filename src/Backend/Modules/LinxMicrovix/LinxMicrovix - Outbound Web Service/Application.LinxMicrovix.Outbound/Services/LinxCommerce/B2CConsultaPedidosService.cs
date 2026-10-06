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
    public class B2CConsultaPedidosService
        : B2CBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidos, B2CConsultaPedidos>,
          IB2CConsultaPedidosService
    {
        private readonly IB2CConsultaPedidosRepository _b2cConsultaPedidosRepository;
        private readonly IB2CConsultaPedidosCommandHandler _b2cConsultaPedidosCommandHandler;

        public B2CConsultaPedidosService(
            IAPICall apiCall,
            ILoggerService logger,
            IIntegrationsCoreRepository coreRepository,
            ILinxMicrovixService linxMicrovixServiceBase,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidos> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IB2CConsultaPedidosRepository b2cConsultaPedidosRepository,
            IB2CConsultaPedidosCommandHandler b2cConsultaPedidosCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _b2cConsultaPedidosRepository = b2cConsultaPedidosRepository;
            _b2cConsultaPedidosCommandHandler = b2cConsultaPedidosCommandHandler;
        }

        protected override EnumJob JobName => EnumJob.B2CConsultaPedidos;
        protected override string CacheKey => "B2CConsultaPedidos:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidos CreateDto(Dictionary<string?, string?> record) =>
            new(
                id_pedido: record.Where(pair => pair.Key == "id_pedido").Select(pair => pair.Value).FirstOrDefault(),
                dt_pedido: record.Where(pair => pair.Key == "dt_pedido").Select(pair => pair.Value).FirstOrDefault(),
                cod_cliente_erp: record.Where(pair => pair.Key == "cod_cliente_erp").Select(pair => pair.Value).FirstOrDefault(),
                cod_cliente_b2c: record.Where(pair => pair.Key == "cod_cliente_b2c").Select(pair => pair.Value).FirstOrDefault(),
                vl_frete: record.Where(pair => pair.Key == "vl_frete").Select(pair => pair.Value).FirstOrDefault(),
                forma_pgto: record.Where(pair => pair.Key == "forma_pgto").Select(pair => pair.Value).FirstOrDefault(),
                plano_pagamento: record.Where(pair => pair.Key == "plano_pagamento").Select(pair => pair.Value).FirstOrDefault(),
                anotacao: record.Where(pair => pair.Key == "anotacao").Select(pair => pair.Value).FirstOrDefault(),
                taxa_impressao: record.Where(pair => pair.Key == "taxa_impressao").Select(pair => pair.Value).FirstOrDefault(),
                finalizado: record.Where(pair => pair.Key == "finalizado").Select(pair => pair.Value).FirstOrDefault(),
                valor_frete_gratis: record.Where(pair => pair.Key == "valor_frete_gratis").Select(pair => pair.Value).FirstOrDefault(),
                tipo_frete: record.Where(pair => pair.Key == "tipo_frete").Select(pair => pair.Value).FirstOrDefault(),
                id_status: record.Where(pair => pair.Key == "id_status").Select(pair => pair.Value).FirstOrDefault(),
                cod_transportador: record.Where(pair => pair.Key == "cod_transportador").Select(pair => pair.Value).FirstOrDefault(),
                tipo_cobranca_frete: record.Where(pair => pair.Key == "tipo_cobranca_frete").Select(pair => pair.Value).FirstOrDefault(),
                ativo: record.Where(pair => pair.Key == "ativo").Select(pair => pair.Value).FirstOrDefault(),
                empresa: record.Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                id_tabela_preco: record.Where(pair => pair.Key == "id_tabela_preco").Select(pair => pair.Value).FirstOrDefault(),
                valor_credito: record.Where(pair => pair.Key == "valor_credito").Select(pair => pair.Value).FirstOrDefault(),
                cod_vendedor: record.Where(pair => pair.Key == "cod_vendedor").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                dt_disponivel_faturamento: record.Where(pair => pair.Key == "dt_disponivel_faturamento").Select(pair => pair.Value).FirstOrDefault(),
                dt_insert: record.Where(pair => pair.Key == "dt_insert").Select(pair => pair.Value).FirstOrDefault(),
                mensagem_falha_faturamento: record.Where(pair => pair.Key == "mensagem_falha_faturamento").Select(pair => pair.Value).FirstOrDefault(),
                id_tipo_b2c: record.Where(pair => pair.Key == "id_tipo_b2c").Select(pair => pair.Value).FirstOrDefault(),
                ecommerce_origem: record.Where(pair => pair.Key == "ecommerce_origem").Select(pair => pair.Value).FirstOrDefault(),
                order_id: record.Where(pair => pair.Key == "order_id").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override B2CConsultaPedidos CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidos dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"id_pedido: {record.Where(p => p.Key == "id_pedido").Select(p => p.Value).FirstOrDefault()} | order_id: {record.Where(p => p.Key == "order_id").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(B2CConsultaPedidos entity) => entity.recordKey;
        protected override string? GetRecordXml(B2CConsultaPedidos entity) => entity.recordXml;

        protected override string BuildEntityLogContext(B2CConsultaPedidos entity) =>
            $"id_pedido: {entity.id_pedido} | order_id: {entity.order_id}";

        protected override string BuildRequestBodyParameters(string parameters, LinxAPIParam jobParameter) =>
            parameters
                .Replace("[0]", "0")
                .Replace("[data_origem_inicial]", $"{DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}")
                .Replace("[data_origem_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}");

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<B2CConsultaPedidos> records) =>
            await _b2cConsultaPedidosRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.id_pedido).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<B2CConsultaPedidos> records, LinxAPIParam jobParameter) =>
            _b2cConsultaPedidosRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<B2CConsultaPedidos?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<B2CConsultaPedidos?>().ToList();

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
                    await _b2cConsultaPedidosRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> IntegrityLockRegisters(LinxAPIParam jobParameter)
        {
            try
            {
                var _listSomenteNovos = new List<B2CConsultaPedidos>();

                string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
                string? parameters = await _coreRepository.GetRecord<string>(sql);

                string integritySql = _b2cConsultaPedidosCommandHandler.CreateIntegrityLockQuery();
                var _listRegistrosConsultados = await _coreRepository.GetRecords<IntegrityLockTablesRegister>(integritySql);

                foreach (var pedido in _listRegistrosConsultados)
                {
                    var bodyLists = new List<string?>();

                    bodyLists.Add(_linxMicrovixServiceBase.BuildBodyRequest(
                                    parametersList: parameters.Replace("[0]", "0").Replace("[order_id]", pedido.identifier),
                                    jobParameter: jobParameter,
                                    cnpj_emp: jobParameter.DocMainCompany
                                ));

                    bodyLists.Add(_linxMicrovixServiceBase.BuildBodyRequest(
                                    parametersList: parameters.Replace("[0]", "0").Replace("[order_id]", $"{pedido.identifier}-CANCELLED"),
                                    jobParameter: jobParameter,
                                    cnpj_emp: jobParameter.DocMainCompany
                                ));

                    foreach (var body in bodyLists)
                    {
                        string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                        var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

                        if (xmls.Count() > 0)
                        {
                            var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);
                            _listSomenteNovos.AddRange(listRecords);
                            pedido.is_present_in_erp = true;
                        }
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
