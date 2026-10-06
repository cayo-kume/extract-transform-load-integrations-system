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
    public class LinxMovimentoService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimento, LinxMovimento>,
          ILinxMovimentoService
    {
        private readonly ILinxMovimentoRepository _linxMovimentoRepository;

        public LinxMovimentoService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxMovimentoRepository linxMovimentoRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimento> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxMovimentoRepository = linxMovimentoRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxMovimento;
        protected override string CacheKey => "LinxMovimento:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimento CreateDto(Dictionary<string?, string?> record) =>
            new(
                cnpj_emp: record.Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                transacao: record.Where(pair => pair.Key == "transacao").Select(pair => pair.Value).FirstOrDefault(),
                usuario: record.Where(pair => pair.Key == "usuario").Select(pair => pair.Value).FirstOrDefault(),
                documento: record.Where(pair => pair.Key == "documento").Select(pair => pair.Value).FirstOrDefault(),
                chave_nf: record.Where(pair => pair.Key == "chave_nf").Select(pair => pair.Value).FirstOrDefault(),
                ecf: record.Where(pair => pair.Key == "ecf").Select(pair => pair.Value).FirstOrDefault(),
                numero_serie_ecf: record.Where(pair => pair.Key == "numero_serie_ecf").Select(pair => pair.Value).FirstOrDefault(),
                modelo_nf: record.Where(pair => pair.Key == "modelo_nf").Select(pair => pair.Value).FirstOrDefault(),
                data_documento: record.Where(pair => pair.Key == "data_documento").Select(pair => pair.Value).FirstOrDefault(),
                data_lancamento: record.Where(pair => pair.Key == "data_lancamento").Select(pair => pair.Value).FirstOrDefault(),
                codigo_cliente: record.Where(pair => pair.Key == "codigo_cliente").Select(pair => pair.Value).FirstOrDefault(),
                serie: record.Where(pair => pair.Key == "serie").Select(pair => pair.Value).FirstOrDefault(),
                desc_cfop: record.Where(pair => pair.Key == "desc_cfop").Select(pair => pair.Value).FirstOrDefault(),
                id_cfop: record.Where(pair => pair.Key == "id_cfop").Select(pair => pair.Value).FirstOrDefault(),
                cod_vendedor: record.Where(pair => pair.Key == "cod_vendedor").Select(pair => pair.Value).FirstOrDefault(),
                quantidade: record.Where(pair => pair.Key == "quantidade").Select(pair => pair.Value).FirstOrDefault(),
                preco_custo: record.Where(pair => pair.Key == "preco_custo").Select(pair => pair.Value).FirstOrDefault(),
                valor_liquido: record.Where(pair => pair.Key == "valor_liquido").Select(pair => pair.Value).FirstOrDefault(),
                desconto: record.Where(pair => pair.Key == "desconto").Select(pair => pair.Value).FirstOrDefault(),
                cst_icms: record.Where(pair => pair.Key == "cst_icms").Select(pair => pair.Value).FirstOrDefault(),
                cst_pis: record.Where(pair => pair.Key == "cst_pis").Select(pair => pair.Value).FirstOrDefault(),
                cst_cofins: record.Where(pair => pair.Key == "cst_cofins").Select(pair => pair.Value).FirstOrDefault(),
                cst_ipi: record.Where(pair => pair.Key == "cst_ipi").Select(pair => pair.Value).FirstOrDefault(),
                valor_icms: record.Where(pair => pair.Key == "valor_icms").Select(pair => pair.Value).FirstOrDefault(),
                aliquota_icms: record.Where(pair => pair.Key == "aliquota_icms").Select(pair => pair.Value).FirstOrDefault(),
                aliquota_icms_st: record.Where(pair => pair.Key == "aliquota_icms_st").Select(pair => pair.Value).FirstOrDefault(),
                base_icms: record.Where(pair => pair.Key == "base_icms").Select(pair => pair.Value).FirstOrDefault(),
                valor_pis: record.Where(pair => pair.Key == "valor_pis").Select(pair => pair.Value).FirstOrDefault(),
                aliquota_pis: record.Where(pair => pair.Key == "aliquota_pis").Select(pair => pair.Value).FirstOrDefault(),
                base_pis: record.Where(pair => pair.Key == "base_pis").Select(pair => pair.Value).FirstOrDefault(),
                valor_cofins: record.Where(pair => pair.Key == "valor_cofins").Select(pair => pair.Value).FirstOrDefault(),
                aliquota_cofins: record.Where(pair => pair.Key == "aliquota_cofins").Select(pair => pair.Value).FirstOrDefault(),
                base_cofins: record.Where(pair => pair.Key == "base_cofins").Select(pair => pair.Value).FirstOrDefault(),
                valor_icms_st: record.Where(pair => pair.Key == "valor_icms_st").Select(pair => pair.Value).FirstOrDefault(),
                base_icms_st: record.Where(pair => pair.Key == "base_icms_st").Select(pair => pair.Value).FirstOrDefault(),
                valor_ipi: record.Where(pair => pair.Key == "valor_ipi").Select(pair => pair.Value).FirstOrDefault(),
                aliquota_ipi: record.Where(pair => pair.Key == "aliquota_ipi").Select(pair => pair.Value).FirstOrDefault(),
                base_ipi: record.Where(pair => pair.Key == "base_ipi").Select(pair => pair.Value).FirstOrDefault(),
                valor_total: record.Where(pair => pair.Key == "valor_total").Select(pair => pair.Value).FirstOrDefault(),
                forma_dinheiro: record.Where(pair => pair.Key == "forma_dinheiro").Select(pair => pair.Value).FirstOrDefault(),
                total_dinheiro: record.Where(pair => pair.Key == "total_dinheiro").Select(pair => pair.Value).FirstOrDefault(),
                forma_cheque: record.Where(pair => pair.Key == "forma_cheque").Select(pair => pair.Value).FirstOrDefault(),
                total_cheque: record.Where(pair => pair.Key == "total_cheque").Select(pair => pair.Value).FirstOrDefault(),
                forma_cartao: record.Where(pair => pair.Key == "forma_cartao").Select(pair => pair.Value).FirstOrDefault(),
                total_cartao: record.Where(pair => pair.Key == "total_cartao").Select(pair => pair.Value).FirstOrDefault(),
                forma_crediario: record.Where(pair => pair.Key == "forma_crediario").Select(pair => pair.Value).FirstOrDefault(),
                total_crediario: record.Where(pair => pair.Key == "total_crediario").Select(pair => pair.Value).FirstOrDefault(),
                forma_convenio: record.Where(pair => pair.Key == "forma_convenio").Select(pair => pair.Value).FirstOrDefault(),
                total_convenio: record.Where(pair => pair.Key == "total_convenio").Select(pair => pair.Value).FirstOrDefault(),
                frete: record.Where(pair => pair.Key == "frete").Select(pair => pair.Value).FirstOrDefault(),
                operacao: record.Where(pair => pair.Key == "operacao").Select(pair => pair.Value).FirstOrDefault(),
                tipo_transacao: record.Where(pair => pair.Key == "tipo_transacao").Select(pair => pair.Value).FirstOrDefault(),
                cod_produto: record.Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault(),
                cod_barra: record.Where(pair => pair.Key == "cod_barra").Select(pair => pair.Value).FirstOrDefault(),
                cancelado: record.Where(pair => pair.Key == "cancelado").Select(pair => pair.Value).FirstOrDefault(),
                excluido: record.Where(pair => pair.Key == "excluido").Select(pair => pair.Value).FirstOrDefault(),
                soma_relatorio: record.Where(pair => pair.Key == "soma_relatorio").Select(pair => pair.Value).FirstOrDefault(),
                identificador: record.Where(pair => pair.Key == "identificador").Select(pair => pair.Value).FirstOrDefault(),
                deposito: record.Where(pair => pair.Key == "deposito").Select(pair => pair.Value).FirstOrDefault(),
                obs: record.Where(pair => pair.Key == "obs").Select(pair => pair.Value).FirstOrDefault(),
                preco_unitario: record.Where(pair => pair.Key == "preco_unitario").Select(pair => pair.Value).FirstOrDefault(),
                hora_lancamento: record.Where(pair => pair.Key == "hora_lancamento").Select(pair => pair.Value).FirstOrDefault(),
                natureza_operacao: record.Where(pair => pair.Key == "natureza_operacao").Select(pair => pair.Value).FirstOrDefault(),
                tabela_preco: record.Where(pair => pair.Key == "tabela_preco").Select(pair => pair.Value).FirstOrDefault(),
                nome_tabela_preco: record.Where(pair => pair.Key == "nome_tabela_preco").Select(pair => pair.Value).FirstOrDefault(),
                cod_sefaz_situacao: record.Where(pair => pair.Key == "cod_sefaz_situacao").Select(pair => pair.Value).FirstOrDefault(),
                desc_sefaz_situacao: record.Where(pair => pair.Key == "desc_sefaz_situacao").Select(pair => pair.Value).FirstOrDefault(),
                protocolo_aut_nfe: record.Where(pair => pair.Key == "protocolo_aut_nfe").Select(pair => pair.Value).FirstOrDefault(),
                dt_update: record.Where(pair => pair.Key == "dt_update").Select(pair => pair.Value).FirstOrDefault(),
                forma_cheque_prazo: record.Where(pair => pair.Key == "forma_cheque_prazo").Select(pair => pair.Value).FirstOrDefault(),
                total_cheque_prazo: record.Where(pair => pair.Key == "total_cheque_prazo").Select(pair => pair.Value).FirstOrDefault(),
                cod_natureza_operacao: record.Where(pair => pair.Key == "cod_natureza_operacao").Select(pair => pair.Value).FirstOrDefault(),
                preco_tabela_epoca: record.Where(pair => pair.Key == "preco_tabela_epoca").Select(pair => pair.Value).FirstOrDefault(),
                desconto_total_item: record.Where(pair => pair.Key == "desconto_total_item").Select(pair => pair.Value).FirstOrDefault(),
                conferido: record.Where(pair => pair.Key == "conferido").Select(pair => pair.Value).FirstOrDefault(),
                transacao_pedido_venda: record.Where(pair => pair.Key == "transacao_pedido_venda").Select(pair => pair.Value).FirstOrDefault(),
                codigo_modelo_nf: record.Where(pair => pair.Key == "codigo_modelo_nf").Select(pair => pair.Value).FirstOrDefault(),
                acrescimo: record.Where(pair => pair.Key == "acrescimo").Select(pair => pair.Value).FirstOrDefault(),
                mob_checkout: record.Where(pair => pair.Key == "mob_checkout").Select(pair => pair.Value).FirstOrDefault(),
                aliquota_iss: record.Where(pair => pair.Key == "aliquota_iss").Select(pair => pair.Value).FirstOrDefault(),
                base_iss: record.Where(pair => pair.Key == "base_iss").Select(pair => pair.Value).FirstOrDefault(),
                ordem: record.Where(pair => pair.Key == "ordem").Select(pair => pair.Value).FirstOrDefault(),
                codigo_rotina_origem: record.Where(pair => pair.Key == "codigo_rotina_origem").Select(pair => pair.Value).FirstOrDefault(),
                troco: record.Where(pair => pair.Key == "troco").Select(pair => pair.Value).FirstOrDefault(),
                transportador: record.Where(pair => pair.Key == "transportador").Select(pair => pair.Value).FirstOrDefault(),
                icms_aliquota_desonerado: record.Where(pair => pair.Key == "icms_aliquota_desonerado").Select(pair => pair.Value).FirstOrDefault(),
                icms_valor_desonerado_item: record.Where(pair => pair.Key == "icms_valor_desonerado_item").Select(pair => pair.Value).FirstOrDefault(),
                empresa: record.Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                desconto_item: record.Where(pair => pair.Key == "desconto_item").Select(pair => pair.Value).FirstOrDefault(),
                aliq_iss: record.Where(pair => pair.Key == "aliq_iss").Select(pair => pair.Value).FirstOrDefault(),
                iss_base_item: record.Where(pair => pair.Key == "iss_base_item").Select(pair => pair.Value).FirstOrDefault(),
                despesas: record.Where(pair => pair.Key == "despesas").Select(pair => pair.Value).FirstOrDefault(),
                seguro_total_item: record.Where(pair => pair.Key == "seguro_total_item").Select(pair => pair.Value).FirstOrDefault(),
                acrescimo_total_item: record.Where(pair => pair.Key == "acrescimo_total_item").Select(pair => pair.Value).FirstOrDefault(),
                despesas_total_item: record.Where(pair => pair.Key == "despesas_total_item").Select(pair => pair.Value).FirstOrDefault(),
                forma_pix: record.Where(pair => pair.Key == "forma_pix").Select(pair => pair.Value).FirstOrDefault(),
                total_pix: record.Where(pair => pair.Key == "total_pix").Select(pair => pair.Value).FirstOrDefault(),
                forma_deposito_bancario: record.Where(pair => pair.Key == "forma_deposito_bancario").Select(pair => pair.Value).FirstOrDefault(),
                total_deposito_bancario: record.Where(pair => pair.Key == "total_deposito_bancario").Select(pair => pair.Value).FirstOrDefault(),
                id_venda_produto_b2c: record.Where(pair => pair.Key == "id_venda_produto_b2c").Select(pair => pair.Value).FirstOrDefault(),
                item_promocional: record.Where(pair => pair.Key == "item_promocional").Select(pair => pair.Value).FirstOrDefault(),
                acrescimo_item: record.Where(pair => pair.Key == "acrescimo_item").Select(pair => pair.Value).FirstOrDefault(),
                icms_st_antecipado_aliquota: record.Where(pair => pair.Key == "icms_st_antecipado_aliquota").Select(pair => pair.Value).FirstOrDefault(),
                icms_st_antecipado_margem: record.Where(pair => pair.Key == "icms_st_antecipado_margem").Select(pair => pair.Value).FirstOrDefault(),
                icms_st_antecipado_percentual_reducao: record.Where(pair => pair.Key == "icms_st_antecipado_percentual_reducao").Select(pair => pair.Value).FirstOrDefault(),
                icms_st_antecipado_valor_item: record.Where(pair => pair.Key == "icms_st_antecipado_valor_item").Select(pair => pair.Value).FirstOrDefault(),
                icms_base_desonerado_item: record.Where(pair => pair.Key == "icms_base_desonerado_item").Select(pair => pair.Value).FirstOrDefault(),
                codigo_status_nfe: record.Where(pair => pair.Key == "codigo_status_nfe").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxMovimento CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimento dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"documento: {record.Where(p => p.Key == "documento").Select(p => p.Value).FirstOrDefault()} | cod_produto: {record.Where(p => p.Key == "cod_produto").Select(p => p.Value).FirstOrDefault()} | cnpj_emp: {record.Where(p => p.Key == "cnpj_emp").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxMovimento entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxMovimento entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxMovimento entity) =>
            $"documento: {entity.documento} | cod_produto: {entity.cod_produto} | cnpj_emp: {entity.cnpj_emp}";

        // CRITICAL FISCAL: preserve the GroupBy(identificador) + dedupe logic exactly as the original.
        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxMovimento> records) =>
            await _linxMovimentoRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records
                                .GroupBy(y => y.identificador)
                                .Select(x => (LinxMovimento?)x.First())
                                .ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxMovimento> records, LinxAPIParam jobParameter) =>
            _linxMovimentoRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxMovimento> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records);

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters
                    .Replace("[0]", "0")
                    .Replace("[data_inicial]", "2000-01-01")
                    .Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}")
                    .Replace("[documento]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: cnpj_emp);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxMovimentoRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            var xmls = new List<Dictionary<string?, string?>>();
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var cnpjs_emp = await _coreRepository.GetRecords<Company>(_linxMicrovixCommandHandler.CreateGetMicrovixCompanysQuery());

            foreach (var cnpj_emp in cnpjs_emp)
            {
                var body = _linxMicrovixServiceBase.BuildBodyRequest(
                            parametersList: parameters
                                .Replace("[0]", "0")
                                .Replace("[data_inicial]", $"{DateTime.Today.AddDays(-3).ToString("yyyy-MM-dd")}")
                                .Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
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
