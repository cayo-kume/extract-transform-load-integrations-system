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
    public class LinxMovimentoCartoesService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoCartoes, LinxMovimentoCartoes>,
          ILinxMovimentoCartoesService
    {
        private readonly ILinxMovimentoCartoesRepository _linxMovimentoCartoesRepository;

        public LinxMovimentoCartoesService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxMovimentoCartoesRepository linxMovimentoCartoesRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoCartoes> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxMovimentoCartoesRepository = linxMovimentoCartoesRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxMovimentoCartoes;
        protected override string CacheKey => "LinxMovimentoCartoes:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoCartoes CreateDto(Dictionary<string?, string?> record) =>
            new(
                identificador: record.Where(pair => pair.Key == "identificador").Select(pair => pair.Value).FirstOrDefault(),
                cnpj_emp: record.Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                codlojasitef: record.Where(pair => pair.Key == "codlojasitef").Select(pair => pair.Value).FirstOrDefault(),
                data_lancamento: record.Where(pair => pair.Key == "data_lancamento").Select(pair => pair.Value).FirstOrDefault(),
                cupomfiscal: record.Where(pair => pair.Key == "cupomfiscal").Select(pair => pair.Value).FirstOrDefault(),
                credito_debito: record.Where(pair => pair.Key == "credito_debito").Select(pair => pair.Value).FirstOrDefault(),
                id_cartao_bandeira: record.Where(pair => pair.Key == "id_cartao_bandeira").Select(pair => pair.Value).FirstOrDefault(),
                descricao_bandeira: record.Where(pair => pair.Key == "descricao_bandeira").Select(pair => pair.Value).FirstOrDefault(),
                valor: record.Where(pair => pair.Key == "valor").Select(pair => pair.Value).FirstOrDefault(),
                ordem_cartao: record.Where(pair => pair.Key == "ordem_cartao").Select(pair => pair.Value).FirstOrDefault(),
                nsu_host: record.Where(pair => pair.Key == "nsu_host").Select(pair => pair.Value).FirstOrDefault(),
                nsu_sitef: record.Where(pair => pair.Key == "nsu_sitef").Select(pair => pair.Value).FirstOrDefault(),
                cod_autorizacao: record.Where(pair => pair.Key == "cod_autorizacao").Select(pair => pair.Value).FirstOrDefault(),
                id_antecipacoes_financeiras: record.Where(pair => pair.Key == "id_antecipacoes_financeiras").Select(pair => pair.Value).FirstOrDefault(),
                transacao_servico_terceiro: record.Where(pair => pair.Key == "transacao_servico_terceiro").Select(pair => pair.Value).FirstOrDefault(),
                texto_comprovante: record.Where(pair => pair.Key == "texto_comprovante").Select(pair => pair.Value).FirstOrDefault(),
                id_maquineta_pos: record.Where(pair => pair.Key == "id_maquineta_pos").Select(pair => pair.Value).FirstOrDefault(),
                descricao_maquineta: record.Where(pair => pair.Key == "descricao_maquineta").Select(pair => pair.Value).FirstOrDefault(),
                serie_maquineta: record.Where(pair => pair.Key == "serie_maquineta").Select(pair => pair.Value).FirstOrDefault(),
                cartao_prepago: record.Where(pair => pair.Key == "cartao_prepago").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxMovimentoCartoes CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoCartoes dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"identificador: {record.Where(p => p.Key == "identificador").Select(p => p.Value).FirstOrDefault()} | cupomfiscal: {record.Where(p => p.Key == "cupomfiscal").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxMovimentoCartoes entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxMovimentoCartoes entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxMovimentoCartoes entity) =>
            $"identificador: {entity.identificador} | cupomfiscal: {entity.cupomfiscal}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxMovimentoCartoes> records) =>
            await _linxMovimentoCartoesRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records
                            .GroupBy(y => y.identificador)
                            .Select(x => x.First())
                            .Cast<LinxMovimentoCartoes?>()
                            .ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxMovimentoCartoes> records, LinxAPIParam jobParameter) =>
            _linxMovimentoCartoesRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxMovimentoCartoes?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxMovimentoCartoes?>().ToList();

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            var sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.jobName, identificador, cnpj_emp);
            var result = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: result.Replace("[0]", "0").Replace("[identificador]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: cnpj_emp);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxMovimentoCartoesRepository.InsertRecord(record: record, jobParameter: jobParameter);
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

            string sqlCnpjsEmp = _linxMicrovixCommandHandler.CreateGetB2CCompanysQuery();
            var cnpjs_emp = await _coreRepository.GetRecords<Company?>(sql);

            foreach (var cnpj_emp in cnpjs_emp)
            {
                string? timestamp = _linxMicrovixCommandHandler.CreateGetLast7DaysMinTimestampWithCompanyQuery(
                    jobParameter.schema,
                    jobParameter.tableName,
                    columnDate: "data_lancamento",
                    columnCompany: "cnpj_emp",
                    companyValue: cnpj_emp.Doc
                );

                var body = _linxMicrovixServiceBase.BuildBodyRequest(
                    parametersList: parameters.Replace("[0]", timestamp).Replace("[data_inicial]", $"{DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd")}").Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
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
