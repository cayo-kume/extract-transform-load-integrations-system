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
    public class LinxPedidosVendaService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPedidosVenda, LinxPedidosVenda>,
          ILinxPedidosVendaService
    {
        private readonly ILinxPedidosVendaRepository _linxPedidosVendaRepository;

        public LinxPedidosVendaService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            ILinxPedidosVendaRepository linxPedidosVendaRepository,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPedidosVenda> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxPedidosVendaRepository = linxPedidosVendaRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxPedidosVenda;
        protected override string CacheKey => "LinxPedidosVenda:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPedidosVenda CreateDto(Dictionary<string?, string?> record) =>
            new(
                cnpj_emp: record.Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                cod_pedido: record.Where(pair => pair.Key == "cod_pedido").Select(pair => pair.Value).FirstOrDefault(),
                data_lancamento: record.Where(pair => pair.Key == "data_lancamento").Select(pair => pair.Value).FirstOrDefault(),
                hora_lancamento: record.Where(pair => pair.Key == "hora_lancamento").Select(pair => pair.Value).FirstOrDefault(),
                transacao: record.Where(pair => pair.Key == "transacao").Select(pair => pair.Value).FirstOrDefault(),
                usuario: record.Where(pair => pair.Key == "usuario").Select(pair => pair.Value).FirstOrDefault(),
                codigo_cliente: record.Where(pair => pair.Key == "codigo_cliente").Select(pair => pair.Value).FirstOrDefault(),
                cod_produto: record.Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault(),
                quantidade: record.Where(pair => pair.Key == "quantidade").Select(pair => pair.Value).FirstOrDefault(),
                valor_unitario: record.Where(pair => pair.Key == "valor_unitario").Select(pair => pair.Value).FirstOrDefault(),
                cod_vendedor: record.Where(pair => pair.Key == "cod_vendedor").Select(pair => pair.Value).FirstOrDefault(),
                valor_frete: record.Where(pair => pair.Key == "valor_frete").Select(pair => pair.Value).FirstOrDefault(),
                valor_total: record.Where(pair => pair.Key == "valor_total").Select(pair => pair.Value).FirstOrDefault(),
                desconto_item: record.Where(pair => pair.Key == "desconto_item").Select(pair => pair.Value).FirstOrDefault(),
                cod_plano_pagamento: record.Where(pair => pair.Key == "cod_plano_pagamento").Select(pair => pair.Value).FirstOrDefault(),
                plano_pagamento: record.Where(pair => pair.Key == "plano_pagamento").Select(pair => pair.Value).FirstOrDefault(),
                obs: record.Where(pair => pair.Key == "obs").Select(pair => pair.Value).FirstOrDefault(),
                aprovado: record.Where(pair => pair.Key == "aprovado").Select(pair => pair.Value).FirstOrDefault(),
                cancelado: record.Where(pair => pair.Key == "cancelado").Select(pair => pair.Value).FirstOrDefault(),
                data_aprovacao: record.Where(pair => pair.Key == "data_aprovacao").Select(pair => pair.Value).FirstOrDefault(),
                data_alteracao: record.Where(pair => pair.Key == "data_alteracao").Select(pair => pair.Value).FirstOrDefault(),
                tipo_frete: record.Where(pair => pair.Key == "tipo_frete").Select(pair => pair.Value).FirstOrDefault(),
                natureza_operacao: record.Where(pair => pair.Key == "natureza_operacao").Select(pair => pair.Value).FirstOrDefault(),
                tabela_preco: record.Where(pair => pair.Key == "tabela_preco").Select(pair => pair.Value).FirstOrDefault(),
                nome_tabela_preco: record.Where(pair => pair.Key == "nome_tabela_preco").Select(pair => pair.Value).FirstOrDefault(),
                previsao_entrega: record.Where(pair => pair.Key == "previsao_entrega").Select(pair => pair.Value).FirstOrDefault(),
                realizado_por: record.Where(pair => pair.Key == "realizado_por").Select(pair => pair.Value).FirstOrDefault(),
                pontuacao_ser: record.Where(pair => pair.Key == "pontuacao_ser").Select(pair => pair.Value).FirstOrDefault(),
                venda_externa: record.Where(pair => pair.Key == "venda_externa").Select(pair => pair.Value).FirstOrDefault(),
                nf_gerada: record.Where(pair => pair.Key == "nf_gerada").Select(pair => pair.Value).FirstOrDefault(),
                status: record.Where(pair => pair.Key == "status").Select(pair => pair.Value).FirstOrDefault(),
                numero_projeto_officina: record.Where(pair => pair.Key == "numero_projeto_officina").Select(pair => pair.Value).FirstOrDefault(),
                cod_natureza_operacao: record.Where(pair => pair.Key == "cod_natureza_operacao").Select(pair => pair.Value).FirstOrDefault(),
                margem_contribuicao: record.Where(pair => pair.Key == "margem_contribuicao").Select(pair => pair.Value).FirstOrDefault(),
                doc_origem: record.Where(pair => pair.Key == "doc_origem").Select(pair => pair.Value).FirstOrDefault(),
                posicao_item: record.Where(pair => pair.Key == "posicao_item").Select(pair => pair.Value).FirstOrDefault(),
                orcamento_origem: record.Where(pair => pair.Key == "orcamento_origem").Select(pair => pair.Value).FirstOrDefault(),
                transacao_origem: record.Where(pair => pair.Key == "transacao_origem").Select(pair => pair.Value).FirstOrDefault(),
                desconto: record.Where(pair => pair.Key == "desconto").Select(pair => pair.Value).FirstOrDefault(),
                transacao_ws: record.Where(pair => pair.Key == "transacao_ws").Select(pair => pair.Value).FirstOrDefault(),
                empresa: record.Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                transportador: record.Where(pair => pair.Key == "transportador").Select(pair => pair.Value).FirstOrDefault(),
                deposito: record.Where(pair => pair.Key == "deposito").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxPedidosVenda CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxPedidosVenda dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"cnpj_emp: {record.Where(p => p.Key == "cnpj_emp").Select(p => p.Value).FirstOrDefault()} | cod_pedido: {record.Where(p => p.Key == "cod_pedido").Select(p => p.Value).FirstOrDefault()} | cod_produto: {record.Where(p => p.Key == "cod_produto").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxPedidosVenda entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxPedidosVenda entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxPedidosVenda entity) =>
            $"cnpj_emp: {entity.cnpj_emp} | cod_pedido: {entity.cod_pedido} | cod_produto: {entity.cod_produto}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxPedidosVenda> records) =>
            await _linxPedidosVendaRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records
                                .GroupBy(y => y.cod_pedido)
                                .Select(g => g.First())
                                .Select(x => x.cod_pedido)
                                .ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxPedidosVenda> records, LinxAPIParam jobParameter) =>
            _linxPedidosVendaRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxPedidosVenda?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxPedidosVenda?>().ToList();

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

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[data_inicial]", "2000-01-01").Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}").Replace("[doc_origem]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: cnpj_emp);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxPedidosVendaRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }
    }
}
