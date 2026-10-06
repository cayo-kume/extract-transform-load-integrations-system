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
    public class LinxPedidosCompraService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPedidosCompra, LinxPedidosCompra>,
          ILinxPedidosCompraService
    {
        private readonly ILinxPedidosCompraRepository _linxPedidosCompraRepository;

        public LinxPedidosCompraService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            ILinxPedidosCompraRepository linxPedidosCompraRepository,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPedidosCompra> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxPedidosCompraRepository = linxPedidosCompraRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxPedidosCompra;
        protected override string CacheKey => "LinxPedidosCompra:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPedidosCompra CreateDto(Dictionary<string?, string?> record) =>
            new(
                cnpj_emp: record.Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                cod_pedido: record.Where(pair => pair.Key == "cod_pedido").Select(pair => pair.Value).FirstOrDefault().Replace("-", ""),
                data_pedido: record.Where(pair => pair.Key == "data_pedido").Select(pair => pair.Value).FirstOrDefault(),
                transacao: record.Where(pair => pair.Key == "transacao").Select(pair => pair.Value).FirstOrDefault(),
                usuario: record.Where(pair => pair.Key == "usuario").Select(pair => pair.Value).FirstOrDefault(),
                codigo_fornecedor: record.Where(pair => pair.Key == "codigo_fornecedor").Select(pair => pair.Value).FirstOrDefault(),
                cod_produto: record.Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault(),
                quantidade: record.Where(pair => pair.Key == "quantidade").Select(pair => pair.Value).FirstOrDefault(),
                valor_unitario: record.Where(pair => pair.Key == "valor_unitario").Select(pair => pair.Value).FirstOrDefault(),
                cod_comprador: record.Where(pair => pair.Key == "cod_comprador").Select(pair => pair.Value).FirstOrDefault(),
                valor_frete: record.Where(pair => pair.Key == "valor_frete").Select(pair => pair.Value).FirstOrDefault(),
                valor_total: record.Where(pair => pair.Key == "valor_total").Select(pair => pair.Value).FirstOrDefault(),
                cod_plano_pagamento: record.Where(pair => pair.Key == "cod_plano_pagamento").Select(pair => pair.Value).FirstOrDefault(),
                plano_pagamento: record.Where(pair => pair.Key == "plano_pagamento").Select(pair => pair.Value).FirstOrDefault(),
                obs: record.Where(pair => pair.Key == "obs").Select(pair => pair.Value).FirstOrDefault(),
                aprovado: record.Where(pair => pair.Key == "aprovado").Select(pair => pair.Value).FirstOrDefault(),
                cancelado: record.Where(pair => pair.Key == "cancelado").Select(pair => pair.Value).FirstOrDefault(),
                encerrado: record.Where(pair => pair.Key == "encerrado").Select(pair => pair.Value).FirstOrDefault(),
                data_aprovacao: record.Where(pair => pair.Key == "data_aprovacao").Select(pair => pair.Value).FirstOrDefault(),
                numero_ped_fornec: record.Where(pair => pair.Key == "numero_ped_fornec").Select(pair => pair.Value).FirstOrDefault(),
                tipo_frete: record.Where(pair => pair.Key == "tipo_frete").Select(pair => pair.Value).FirstOrDefault(),
                natureza_operacao: record.Where(pair => pair.Key == "natureza_operacao").Select(pair => pair.Value).FirstOrDefault(),
                previsao_entrega: record.Where(pair => pair.Key == "previsao_entrega").Select(pair => pair.Value).FirstOrDefault(),
                numero_projeto_officina: record.Where(pair => pair.Key == "numero_projeto_officina").Select(pair => pair.Value).FirstOrDefault(),
                status_pedido: record.Where(pair => pair.Key == "status_pedido").Select(pair => pair.Value).FirstOrDefault(),
                qtde_entregue: record.Where(pair => pair.Key == "qtde_entregue").Select(pair => pair.Value).FirstOrDefault(),
                descricao_frete: record.Where(pair => pair.Key == "descricao_frete").Select(pair => pair.Value).FirstOrDefault(),
                integrado_linx: record.Where(pair => pair.Key == "integrado_linx").Select(pair => pair.Value).FirstOrDefault(),
                nf_gerada: record.Where(pair => pair.Key == "nf_gerada").Select(pair => pair.Value).FirstOrDefault(),
                empresa: record.Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                nf_origem_ws: record.Where(pair => pair.Key == "nf_origem_ws").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxPedidosCompra CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPedidosCompra dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"cnpj_emp: {record.Where(p => p.Key == "cnpj_emp").Select(p => p.Value).FirstOrDefault()} | cod_pedido: {record.Where(p => p.Key == "cod_pedido").Select(p => p.Value).FirstOrDefault()} | cod_produto: {record.Where(p => p.Key == "cod_produto").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxPedidosCompra entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxPedidosCompra entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxPedidosCompra entity) =>
            $"cnpj_emp: {entity.cnpj_emp} | cod_pedido: {entity.cod_pedido} | cod_produto: {entity.cod_produto}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxPedidosCompra> records) =>
            await _linxPedidosCompraRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records
                                .GroupBy(x => x.cod_pedido)
                                .Select(g => g.First())
                                .Select(y => y.cod_pedido)
                                .ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxPedidosCompra> records, LinxAPIParam jobParameter) =>
            _linxPedidosCompraRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxPedidosCompra?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxPedidosCompra?>().ToList();

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
                            parametersList: parameters.Replace("[0]", "0").Replace("[data_inicial]", $"{DateTime.Today.AddMonths(-6).ToString("yyyy-MM-dd")}").Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
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

        public async Task<bool> IntegrityLockRegisters(LinxAPIParam jobParameter)
        {
            try
            {
                var xmls = new List<Dictionary<string?, string?>>();

                string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
                string? parameters = await _coreRepository.GetRecord<string>(sql);

                var sqlCnpjs = _linxMicrovixCommandHandler.CreateGetMicrovixCompanysQuery();
                var cnpjs_emp = await _coreRepository.GetRecords<Company>(sqlCnpjs);

                foreach (var cnpj_emp in cnpjs_emp)
                {
                    var body = _linxMicrovixServiceBase.BuildBodyRequest(
                                parametersList: parameters.Replace("[0]", "0").Replace("[data_inicial]", $"{DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}").Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}").Replace("[data_alt_inicial]", $"{DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}").Replace("[data_alt_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
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

                    if (listRecords.Count() > 0)
                    {
                        BulkInsertIntoTableRaw(listRecords, jobParameter);
                        await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
