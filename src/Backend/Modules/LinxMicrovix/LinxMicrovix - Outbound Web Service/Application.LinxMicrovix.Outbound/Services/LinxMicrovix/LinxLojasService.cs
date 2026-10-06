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
    public class LinxLojasService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxLojas, LinxLojas>,
          ILinxLojasService
    {
        private readonly ILinxLojasRepository _linxLojasRepository;

        public LinxLojasService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxLojasRepository linxLojasRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxLojas> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxLojasRepository = linxLojasRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxLojas;
        protected override string CacheKey => "LinxLojas:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxLojas CreateDto(Dictionary<string?, string?> record) =>
            new(
                empresa: record.Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                nome_emp: record.Where(pair => pair.Key == "nome_emp").Select(pair => pair.Value).FirstOrDefault(),
                razao_emp: record.Where(pair => pair.Key == "razao_emp").Select(pair => pair.Value).FirstOrDefault(),
                cnpj_emp: record.Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                inscricao_emp: record.Where(pair => pair.Key == "inscricao_emp").Select(pair => pair.Value).FirstOrDefault(),
                endereco_emp: record.Where(pair => pair.Key == "endereco_emp").Select(pair => pair.Value).FirstOrDefault(),
                num_emp: record.Where(pair => pair.Key == "num_emp").Select(pair => pair.Value).FirstOrDefault(),
                complement_emp: record.Where(pair => pair.Key == "complement_emp").Select(pair => pair.Value).FirstOrDefault(),
                bairro_emp: record.Where(pair => pair.Key == "bairro_emp").Select(pair => pair.Value).FirstOrDefault(),
                cep_emp: record.Where(pair => pair.Key == "cep_emp").Select(pair => pair.Value).FirstOrDefault(),
                cidade_emp: record.Where(pair => pair.Key == "cidade_emp").Select(pair => pair.Value).FirstOrDefault(),
                estado_emp: record.Where(pair => pair.Key == "estado_emp").Select(pair => pair.Value).FirstOrDefault(),
                fone_emp: record.Where(pair => pair.Key == "fone_emp").Select(pair => pair.Value).FirstOrDefault(),
                email_emp: record.Where(pair => pair.Key == "email_emp").Select(pair => pair.Value).FirstOrDefault(),
                cod_ibge_municipio: record.Where(pair => pair.Key == "cod_ibge_municipio").Select(pair => pair.Value).FirstOrDefault(),
                data_criacao_emp: record.Where(pair => pair.Key == "data_criacao_emp").Select(pair => pair.Value).FirstOrDefault(),
                data_criacao_portal: record.Where(pair => pair.Key == "data_criacao_portal").Select(pair => pair.Value).FirstOrDefault(),
                sistema_tributacao: record.Where(pair => pair.Key == "sistema_tributacao").Select(pair => pair.Value).FirstOrDefault(),
                regime_tributario: record.Where(pair => pair.Key == "regime_tributario").Select(pair => pair.Value).FirstOrDefault(),
                area_empresa: record.Where(pair => pair.Key == "area_empresa").Select(pair => pair.Value).FirstOrDefault(),
                sigla_empresa: record.Where(pair => pair.Key == "sigla_empresa").Select(pair => pair.Value).FirstOrDefault(),
                id_classe_fiscal: record.Where(pair => pair.Key == "id_classe_fiscal").Select(pair => pair.Value).FirstOrDefault(),
                centro_distribuicao: record.Where(pair => pair.Key == "centro_distribuicao").Select(pair => pair.Value).FirstOrDefault(),
                inscricao_municipal_emp: record.Where(pair => pair.Key == "inscricao_municipal_emp").Select(pair => pair.Value).FirstOrDefault(),
                cnae_emp: record.Where(pair => pair.Key == "cnae_emp").Select(pair => pair.Value).FirstOrDefault(),
                cod_cliente_linx: record.Where(pair => pair.Key == "cod_cliente_linx").Select(pair => pair.Value).FirstOrDefault(),
                tabela_preco_preferencial: record.Where(pair => pair.Key == "tabela_preco_preferencial").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxLojas CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxLojas dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"cnpj_emp: {record.Where(p => p.Key == "cnpj_emp").Select(p => p.Value).FirstOrDefault()} | nome_emp: {record.Where(p => p.Key == "nome_emp").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxLojas entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxLojas entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxLojas entity) =>
            $"empresa: {entity.empresa} | nome_emp: {entity.nome_emp}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxLojas> records)
        {
            var list = await _linxLojasRepository.GetRegistersExists();
            return list.Cast<string?>();
        }

        protected override void BulkInsertIntoTableRaw(List<LinxLojas> records, LinxAPIParam jobParameter) =>
            _linxLojasRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxLojas?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxLojas?>().ToList();

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            var xmls = new List<Dictionary<string?, string?>>();

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            string sqlCnpjsEmp = _linxMicrovixCommandHandler.CreateGetB2CCompanysQuery();
            var cnpjs_emp = await _coreRepository.GetRecords<Company?>(sql);

            foreach (var cnpj_emp in cnpjs_emp)
            {
                var body = _linxMicrovixServiceBase.BuildBodyRequest(
                            parametersList: parameters.Replace("[0]", "0"),
                            jobParameter: jobParameter,
                            cnpj_emp: cnpj_emp.Doc
                        );

                string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                var result2 = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);
                xmls.AddRange(result2);
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
