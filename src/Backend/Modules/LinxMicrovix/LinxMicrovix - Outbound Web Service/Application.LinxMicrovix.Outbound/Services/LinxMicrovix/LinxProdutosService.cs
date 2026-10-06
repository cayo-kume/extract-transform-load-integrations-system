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
    public class LinxProdutosService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutos, LinxProdutos>,
          ILinxProdutosService
    {
        private readonly ILinxProdutosRepository _linxProdutosRepository;

        public LinxProdutosService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxProdutosRepository linxProdutosRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutos> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxProdutosRepository = linxProdutosRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxProdutos;
        protected override string CacheKey => "LinxProdutos:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutos CreateDto(Dictionary<string?, string?> record) =>
            new(
                cod_produto: record.Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault(),
                cod_barra: record.Where(pair => pair.Key == "cod_barra").Select(pair => pair.Value).FirstOrDefault(),
                nome: record.Where(pair => pair.Key == "nome").Select(pair => pair.Value).FirstOrDefault(),
                ncm: record.Where(pair => pair.Key == "ncm").Select(pair => pair.Value).FirstOrDefault(),
                cest: record.Where(pair => pair.Key == "cest").Select(pair => pair.Value).FirstOrDefault(),
                referencia: record.Where(pair => pair.Key == "referencia").Select(pair => pair.Value).FirstOrDefault(),
                cod_auxiliar: record.Where(pair => pair.Key == "cod_auxiliar").Select(pair => pair.Value).FirstOrDefault(),
                unidade: record.Where(pair => pair.Key == "unidade").Select(pair => pair.Value).FirstOrDefault(),
                desc_cor: record.Where(pair => pair.Key == "desc_cor").Select(pair => pair.Value).FirstOrDefault(),
                desc_tamanho: record.Where(pair => pair.Key == "desc_tamanho").Select(pair => pair.Value).FirstOrDefault(),
                desc_setor: record.Where(pair => pair.Key == "desc_setor").Select(pair => pair.Value).FirstOrDefault(),
                desc_linha: record.Where(pair => pair.Key == "desc_linha").Select(pair => pair.Value).FirstOrDefault(),
                desc_marca: record.Where(pair => pair.Key == "desc_marca").Select(pair => pair.Value).FirstOrDefault(),
                desc_colecao: record.Where(pair => pair.Key == "desc_colecao").Select(pair => pair.Value).FirstOrDefault(),
                dt_update: record.Where(pair => pair.Key == "dt_update").Select(pair => pair.Value).FirstOrDefault(),
                cod_fornecedor: record.Where(pair => pair.Key == "cod_fornecedor").Select(pair => pair.Value).FirstOrDefault(),
                desativado: record.Where(pair => pair.Key == "desativado").Select(pair => pair.Value).FirstOrDefault(),
                desc_espessura: record.Where(pair => pair.Key == "desc_espessura").Select(pair => pair.Value).FirstOrDefault(),
                id_espessura: record.Where(pair => pair.Key == "id_espessura").Select(pair => pair.Value).FirstOrDefault(),
                desc_classificacao: record.Where(pair => pair.Key == "desc_classificacao").Select(pair => pair.Value).FirstOrDefault(),
                id_classificacao: record.Where(pair => pair.Key == "id_classificacao").Select(pair => pair.Value).FirstOrDefault(),
                origem_mercadoria: record.Where(pair => pair.Key == "origem_mercadoria").Select(pair => pair.Value).FirstOrDefault(),
                peso_liquido: record.Where(pair => pair.Key == "peso_liquido").Select(pair => pair.Value).FirstOrDefault(),
                peso_bruto: record.Where(pair => pair.Key == "peso_bruto").Select(pair => pair.Value).FirstOrDefault(),
                id_cor: record.Where(pair => pair.Key == "id_cor").Select(pair => pair.Value).FirstOrDefault(),
                id_tamanho: record.Where(pair => pair.Key == "id_tamanho").Select(pair => pair.Value).FirstOrDefault(),
                id_setor: record.Where(pair => pair.Key == "id_setor").Select(pair => pair.Value).FirstOrDefault(),
                id_linha: record.Where(pair => pair.Key == "id_linha").Select(pair => pair.Value).FirstOrDefault(),
                id_marca: record.Where(pair => pair.Key == "id_marca").Select(pair => pair.Value).FirstOrDefault(),
                id_colecao: record.Where(pair => pair.Key == "id_colecao").Select(pair => pair.Value).FirstOrDefault(),
                dt_inclusao: record.Where(pair => pair.Key == "dt_inclusao").Select(pair => pair.Value).FirstOrDefault(),
                fator_conversao: record.Where(pair => pair.Key == "fator_conversao").Select(pair => pair.Value).FirstOrDefault(),
                codigo_integracao_ws: record.Where(pair => pair.Key == "codigo_integracao_ws").Select(pair => pair.Value).FirstOrDefault(),
                codigo_integracao_reshop: record.Where(pair => pair.Key == "codigo_integracao_reshop").Select(pair => pair.Value).FirstOrDefault(),
                id_produtos_opticos_tipo: record.Where(pair => pair.Key == "id_produtos_opticos_tipo").Select(pair => pair.Value).FirstOrDefault(),
                id_sped_tipo_item: record.Where(pair => pair.Key == "id_sped_tipo_item").Select(pair => pair.Value).FirstOrDefault(),
                componente: record.Where(pair => pair.Key == "componente").Select(pair => pair.Value).FirstOrDefault(),
                altura_para_frete: record.Where(pair => pair.Key == "altura_para_frete").Select(pair => pair.Value).FirstOrDefault(),
                largura_para_frete: record.Where(pair => pair.Key == "largura_para_frete").Select(pair => pair.Value).FirstOrDefault(),
                comprimento_para_frete: record.Where(pair => pair.Key == "comprimento_para_frete").Select(pair => pair.Value).FirstOrDefault(),
                loja_virtual: record.Where(pair => pair.Key == "loja_virtual").Select(pair => pair.Value).FirstOrDefault(),
                cod_comprador: record.Where(pair => pair.Key == "cod_comprador").Select(pair => pair.Value).FirstOrDefault(),
                obrigatorio_identificacao_cliente: record.Where(pair => pair.Key == "obrigatorio_identificacao_cliente").Select(pair => pair.Value).FirstOrDefault(),
                descricao_basica: record.Where(pair => pair.Key == "descricao_basica").Select(pair => pair.Value).FirstOrDefault(),
                curva: record.Where(pair => pair.Key == "curva").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxProdutos CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutos dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"cod_produto: {record.Where(p => p.Key == "cod_produto").Select(p => p.Value).FirstOrDefault()} | nome: {record.Where(p => p.Key == "nome").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxProdutos entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxProdutos entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxProdutos entity) =>
            $"cod_produto: {entity.cod_produto} | nome: {entity.nome}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxProdutos> records) =>
            await _linxProdutosRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.cod_produto).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxProdutos> records, LinxAPIParam jobParameter) =>
            _linxProdutosRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxProdutos?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxProdutos?>().ToList();

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters
                    .Replace("[0]", "0")
                    .Replace("[dt_update_inicio]", "2000-01-01")
                    .Replace("[dt_update_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}")
                    .Replace("[cod_produto]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxProdutosRepository.InsertRecord(record: record, jobParameter: jobParameter);
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

            var ids_setor = await _linxProdutosRepository.GetProductsSetorIds(jobParameter);

            foreach (var id_setor in ids_setor)
            {
                string sqlTimestamp = _linxMicrovixCommandHandler.CreateGetLast7DaysMinTimestampQuery(jobParameter.schema, jobParameter.tableName, "dt_update");
                string? timestamp = await _coreRepository.GetRecord<string>(sqlTimestamp);

                var body = _linxMicrovixServiceBase.BuildBodyRequest(
                        parametersList: parameters
                            .Replace("[0]", timestamp)
                            .Replace("[dt_update_inicio]", $"{DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}")
                            .Replace("[dt_update_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}")
                            .Replace("[id_setor]", id_setor),
                        jobParameter: jobParameter,
                        cnpj_emp: jobParameter.DocMainCompany
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
