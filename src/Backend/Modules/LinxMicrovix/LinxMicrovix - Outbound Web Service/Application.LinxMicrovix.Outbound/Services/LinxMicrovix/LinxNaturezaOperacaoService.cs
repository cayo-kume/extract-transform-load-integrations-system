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
    public class LinxNaturezaOperacaoService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxNaturezaOperacao, LinxNaturezaOperacao>,
          ILinxNaturezaOperacaoService
    {
        private readonly ILinxNaturezaOperacaoRepository _linxNaturezaOperacaoRepository;

        public LinxNaturezaOperacaoService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            ILinxNaturezaOperacaoRepository linxNaturezaOperacaoRepository,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxNaturezaOperacao> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxNaturezaOperacaoRepository = linxNaturezaOperacaoRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxNaturezaOperacao;
        protected override string CacheKey => "LinxNaturezaOperacao:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxNaturezaOperacao CreateDto(Dictionary<string?, string?> record) =>
            new(
                cod_natureza_operacao: record.Where(pair => pair.Key == "cod_natureza_operacao").Select(pair => pair.Value).FirstOrDefault(),
                descricao: record.Where(pair => pair.Key == "descricao").Select(pair => pair.Value).FirstOrDefault(),
                soma_relatorios: record.Where(pair => pair.Key == "soma_relatorios").Select(pair => pair.Value).FirstOrDefault(),
                operacao: record.Where(pair => pair.Key == "operacao").Select(pair => pair.Value).FirstOrDefault(),
                inativa: record.Where(pair => pair.Key == "inativa").Select(pair => pair.Value).FirstOrDefault(),
                calcula_ipi: record.Where(pair => pair.Key == "calcula_ipi").Select(pair => pair.Value).FirstOrDefault(),
                calcula_iss: record.Where(pair => pair.Key == "calcula_iss").Select(pair => pair.Value).FirstOrDefault(),
                calcula_irrf: record.Where(pair => pair.Key == "calcula_irrf").Select(pair => pair.Value).FirstOrDefault(),
                tipo_preco: record.Where(pair => pair.Key == "tipo_preco").Select(pair => pair.Value).FirstOrDefault(),
                atualiza_custo: record.Where(pair => pair.Key == "atualiza_custo").Select(pair => pair.Value).FirstOrDefault(),
                transferencia: record.Where(pair => pair.Key == "transferencia").Select(pair => pair.Value).FirstOrDefault(),
                baixar_estoque: record.Where(pair => pair.Key == "baixar_estoque").Select(pair => pair.Value).FirstOrDefault(),
                consumo_proprio: record.Where(pair => pair.Key == "consumo_proprio").Select(pair => pair.Value).FirstOrDefault(),
                contabiliza_cmv: record.Where(pair => pair.Key == "contabiliza_cmv").Select(pair => pair.Value).FirstOrDefault(),
                despesa: record.Where(pair => pair.Key == "despesa").Select(pair => pair.Value).FirstOrDefault(),
                atualiza_custo_medio: record.Where(pair => pair.Key == "atualiza_custo_medio").Select(pair => pair.Value).FirstOrDefault(),
                exige_nf_origem: record.Where(pair => pair.Key == "exige_nf_origem").Select(pair => pair.Value).FirstOrDefault(),
                integra_contabilidade: record.Where(pair => pair.Key == "integra_contabilidade").Select(pair => pair.Value).FirstOrDefault(),
                id_obs: record.Where(pair => pair.Key == "id_obs").Select(pair => pair.Value).FirstOrDefault(),
                venda_futura: record.Where(pair => pair.Key == "venda_futura").Select(pair => pair.Value).FirstOrDefault(),
                base_icms_considera_ipi: record.Where(pair => pair.Key == "base_icms_considera_ipi").Select(pair => pair.Value).FirstOrDefault(),
                permite_escolha_historico: record.Where(pair => pair.Key == "permite_escolha_historico").Select(pair => pair.Value).FirstOrDefault(),
                import_produtos: record.Where(pair => pair.Key == "import_produtos").Select(pair => pair.Value).FirstOrDefault(),
                deposito_reserva_venda: record.Where(pair => pair.Key == "deposito_reserva_venda").Select(pair => pair.Value).FirstOrDefault(),
                exibe_nfe: record.Where(pair => pair.Key == "exibe_nfe").Select(pair => pair.Value).FirstOrDefault(),
                faturamento_antecipado: record.Where(pair => pair.Key == "faturamento_antecipado").Select(pair => pair.Value).FirstOrDefault(),
                exibir_informacoes_imposto: record.Where(pair => pair.Key == "exibir_informacoes_imposto").Select(pair => pair.Value).FirstOrDefault(),
                gera_garantia_nacional: record.Where(pair => pair.Key == "gera_garantia_nacional").Select(pair => pair.Value).FirstOrDefault(),
                transferencia_deposito: record.Where(pair => pair.Key == "transferencia_deposito").Select(pair => pair.Value).FirstOrDefault(),
                venda_diferencial_aliquota: record.Where(pair => pair.Key == "venda_diferencial_aliquota").Select(pair => pair.Value).FirstOrDefault(),
                insere_obs_pis_cofins: record.Where(pair => pair.Key == "insere_obs_pis_cofins").Select(pair => pair.Value).FirstOrDefault(),
                diferencial_ativo_consumo: record.Where(pair => pair.Key == "diferencial_ativo_consumo").Select(pair => pair.Value).FirstOrDefault(),
                recusa_de: record.Where(pair => pair.Key == "recusa_de").Select(pair => pair.Value).FirstOrDefault(),
                codigo_ws: record.Where(pair => pair.Key == "codigo_ws").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxNaturezaOperacao CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxNaturezaOperacao dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"cod_natureza_operacao: {record.Where(p => p.Key == "cod_natureza_operacao").Select(p => p.Value).FirstOrDefault()} | descricao: {record.Where(p => p.Key == "descricao").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxNaturezaOperacao entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxNaturezaOperacao entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxNaturezaOperacao entity) =>
            $"cod_natureza_operacao: {entity.cod_natureza_operacao} | descricao: {entity.descricao}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxNaturezaOperacao> records) =>
            await _linxNaturezaOperacaoRepository.GetRegistersExists();

        protected override void BulkInsertIntoTableRaw(List<LinxNaturezaOperacao> records, LinxAPIParam jobParameter) =>
            _linxNaturezaOperacaoRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxNaturezaOperacao?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxNaturezaOperacao?>().ToList();

        public Task<bool> GetRecords(LinxAPIParam jobParameter) => GetRecordsCore(jobParameter);

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[cod_natureza_operacao]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxNaturezaOperacaoRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }
    }
}
