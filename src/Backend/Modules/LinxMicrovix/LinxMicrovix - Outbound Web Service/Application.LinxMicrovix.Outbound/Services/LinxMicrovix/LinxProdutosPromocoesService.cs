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
    public class LinxProdutosPromocoesService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosPromocoes, LinxProdutosPromocoes>,
          ILinxProdutosPromocoesService
    {
        private readonly ILinxProdutosPromocoesRepository _linxProdutosPromocoesRepository;

        public LinxProdutosPromocoesService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            ILinxProdutosPromocoesRepository linxProdutosPromocoesRepository,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosPromocoes> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxProdutosPromocoesRepository = linxProdutosPromocoesRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxProdutosPromocoes;
        protected override string CacheKey => "LinxProdutosPromocoes:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosPromocoes CreateDto(Dictionary<string?, string?> record) =>
            new(
                cnpj_emp: record.Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                cod_produto: record.Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault(),
                preco_promocao: record.Where(pair => pair.Key == "preco_promocao").Select(pair => pair.Value).FirstOrDefault(),
                data_inicio_promocao: record.Where(pair => pair.Key == "data_inicio_promocao").Select(pair => pair.Value).FirstOrDefault(),
                data_termino_promocao: record.Where(pair => pair.Key == "data_termino_promocao").Select(pair => pair.Value).FirstOrDefault(),
                data_cadastro_promocao: record.Where(pair => pair.Key == "data_cadastro_promocao").Select(pair => pair.Value).FirstOrDefault(),
                promocao_ativa: record.Where(pair => pair.Key == "promocao_ativa").Select(pair => pair.Value).FirstOrDefault(),
                id_campanha: record.Where(pair => pair.Key == "id_campanha").Select(pair => pair.Value).FirstOrDefault(),
                nome_campanha: record.Where(pair => pair.Key == "nome_campanha").Select(pair => pair.Value).FirstOrDefault(),
                promocao_opcional: record.Where(pair => pair.Key == "promocao_opcional").Select(pair => pair.Value).FirstOrDefault(),
                custo_total_campanha: record.Where(pair => pair.Key == "custo_total_campanha").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxProdutosPromocoes CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosPromocoes dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"cnpj_emp: {record.Where(p => p.Key == "cnpj_emp").Select(p => p.Value).FirstOrDefault()} | cod_produto: {record.Where(p => p.Key == "cod_produto").Select(p => p.Value).FirstOrDefault()} | nome_campanha: {record.Where(p => p.Key == "nome_campanha").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxProdutosPromocoes entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxProdutosPromocoes entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxProdutosPromocoes entity) =>
            $"cnpj_emp: {entity.cnpj_emp} | cod_produto: {entity.cod_produto} | nome_campanha: {entity.nome_campanha}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxProdutosPromocoes> records) =>
            await _linxProdutosPromocoesRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.cod_produto).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxProdutosPromocoes> records, LinxAPIParam jobParameter) =>
            _linxProdutosPromocoesRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxProdutosPromocoes?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxProdutosPromocoes?>().ToList();

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            string[] characters = { "S", "N" };
            var xmls = new List<Dictionary<string?, string?>>();

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var sqlCnpjs = _linxMicrovixCommandHandler.CreateGetMicrovixCompanysQuery();
            var cnpjs_emp = await _coreRepository.GetRecords<Company>(sqlCnpjs);

            foreach (var character in characters)
            {
                foreach (var cnpj_emp in cnpjs_emp)
                {
                    var body = _linxMicrovixServiceBase.BuildBodyRequest(
                                            parametersList: parameters
                                                            .Replace("[data_cad_inicial]", $"{DateTime.Today.ToString("yyyy-MM-dd")}")
                                                            .Replace("[data_cad_fim]", $"{DateTime.Today.AddYears(5).ToString("yyyy-MM-dd")}")
                                                            .Replace("[data_vig_inicial]", $"{DateTime.Today.ToString("yyyy-MM-dd")}")
                                                            .Replace("[data_vig_fim]", $"{DateTime.Today.AddYears(5).ToString("yyyy-MM-dd")}")
                                                            .Replace("[promocao_ativa]", $"{character}"),
                                            jobParameter: jobParameter,
                                            cnpj_emp: cnpj_emp.Doc
                                        );

                    string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                    var result = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);
                    xmls.AddRange(result);
                }
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
