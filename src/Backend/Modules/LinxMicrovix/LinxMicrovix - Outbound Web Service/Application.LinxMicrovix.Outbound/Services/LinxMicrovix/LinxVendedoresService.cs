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
    public class LinxVendedoresService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxVendedores, LinxVendedores>,
          ILinxVendedoresService
    {
        private readonly ILinxVendedoresRepository _linxVendedoresRepository;

        public LinxVendedoresService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxVendedoresRepository linxVendedoresRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxVendedores> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxVendedoresRepository = linxVendedoresRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxVendedores;
        protected override string CacheKey => "LinxVendedores:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxVendedores CreateDto(Dictionary<string?, string?> record) =>
            new(
                cod_vendedor: record.Where(pair => pair.Key == "cod_vendedor").Select(pair => pair.Value).FirstOrDefault(),
                nome_vendedor: record.Where(pair => pair.Key == "nome_vendedor").Select(pair => pair.Value).FirstOrDefault(),
                tipo_vendedor: record.Where(pair => pair.Key == "tipo_vendedor").Select(pair => pair.Value).FirstOrDefault(),
                end_vend_rua: record.Where(pair => pair.Key == "end_vend_rua").Select(pair => pair.Value).FirstOrDefault(),
                end_vend_numero: record.Where(pair => pair.Key == "end_vend_numero").Select(pair => pair.Value).FirstOrDefault(),
                end_vend_complemento: record.Where(pair => pair.Key == "end_vend_complemento").Select(pair => pair.Value).FirstOrDefault(),
                end_vend_bairro: record.Where(pair => pair.Key == "end_vend_bairro").Select(pair => pair.Value).FirstOrDefault(),
                end_vend_cep: record.Where(pair => pair.Key == "end_vend_cep").Select(pair => pair.Value).FirstOrDefault(),
                end_vend_cidade: record.Where(pair => pair.Key == "end_vend_cidade").Select(pair => pair.Value).FirstOrDefault(),
                end_vend_uf: record.Where(pair => pair.Key == "end_vend_uf").Select(pair => pair.Value).FirstOrDefault(),
                fone_vendedor: record.Where(pair => pair.Key == "fone_vendedor").Select(pair => pair.Value).FirstOrDefault(),
                mail_vendedor: record.Where(pair => pair.Key == "mail_vendedor").Select(pair => pair.Value).FirstOrDefault(),
                dt_upd: record.Where(pair => pair.Key == "dt_upd").Select(pair => pair.Value).FirstOrDefault(),
                cpf_vendedor: record.Where(pair => pair.Key == "cpf_vendedor").Select(pair => pair.Value).FirstOrDefault(),
                ativo: record.Where(pair => pair.Key == "ativo").Select(pair => pair.Value).FirstOrDefault(),
                data_admissao: record.Where(pair => pair.Key == "data_admissao").Select(pair => pair.Value).FirstOrDefault(),
                data_saida: record.Where(pair => pair.Key == "data_saida").Select(pair => pair.Value).FirstOrDefault(),
                matricula: record.Where(pair => pair.Key == "matricula").Select(pair => pair.Value).FirstOrDefault(),
                id_tipo_venda: record.Where(pair => pair.Key == "id_tipo_venda").Select(pair => pair.Value).FirstOrDefault(),
                descricao_tipo_venda: record.Where(pair => pair.Key == "descricao_tipo_venda").Select(pair => pair.Value).FirstOrDefault(),
                cargo: record.Where(pair => pair.Key == "cargo").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxVendedores CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxVendedores dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"cod_vendedor: {record.Where(p => p.Key == "cod_vendedor").Select(p => p.Value).FirstOrDefault()} | nome_vendedor: {record.Where(p => p.Key == "nome_vendedor").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxVendedores entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxVendedores entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxVendedores entity) =>
            $"cod_vendedor: {entity.cod_vendedor} | nome_vendedor: {entity.nome_vendedor}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxVendedores> records)
        {
            var list = await _linxVendedoresRepository.GetRegistersExists();
            return list.Cast<string?>();
        }

        protected override void BulkInsertIntoTableRaw(List<LinxVendedores> records, LinxAPIParam jobParameter) =>
            _linxVendedoresRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxVendedores?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxVendedores?>().ToList();

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[cod_vendedor]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: cnpj_emp);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxVendedoresRepository.InsertRecord(record: record, jobParameter: jobParameter);
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
                                .Replace("[data_upd_inicial]", DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"))
                                .Replace("[data_upd_fim]", DateTime.Now.ToString("yyyy-MM-dd")),
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
