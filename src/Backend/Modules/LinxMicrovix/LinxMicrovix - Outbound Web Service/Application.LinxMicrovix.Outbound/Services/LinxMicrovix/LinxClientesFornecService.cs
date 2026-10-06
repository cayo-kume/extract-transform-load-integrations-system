using Application.Core.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
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
    public class LinxClientesFornecService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxClientesFornec, LinxClientesFornec>,
          ILinxClientesFornecService
    {
        private readonly ILinxClientesFornecRepository _linxClientesFornecRepository;
        private readonly ILinxClientesFornecCommandHandler _linxClientesFornecCommandHandler;

        public LinxClientesFornecService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxClientesFornecRepository linxClientesFornecRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxClientesFornec> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            ILinxClientesFornecCommandHandler linxClientesFornecCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxClientesFornecRepository = linxClientesFornecRepository;
            _linxClientesFornecCommandHandler = linxClientesFornecCommandHandler;
        }

        protected override EnumJob JobName => EnumJob.LinxClientesFornec;
        protected override string CacheKey => "LinxClientesFornec:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxClientesFornec CreateDto(Dictionary<string?, string?> record) =>
            new(
                cod_cliente: record.Where(pair => pair.Key == "cod_cliente").Select(pair => pair.Value).FirstOrDefault(),
                razao_cliente: record.Where(pair => pair.Key == "razao_cliente").Select(pair => pair.Value).FirstOrDefault(),
                nome_cliente: record.Where(pair => pair.Key == "nome_cliente").Select(pair => pair.Value).FirstOrDefault(),
                doc_cliente: record.Where(pair => pair.Key == "doc_cliente").Select(pair => pair.Value).FirstOrDefault(),
                tipo_cliente: record.Where(pair => pair.Key == "tipo_cliente").Select(pair => pair.Value).FirstOrDefault(),
                endereco_cliente: record.Where(pair => pair.Key == "endereco_cliente").Select(pair => pair.Value).FirstOrDefault(),
                numero_rua_cliente: record.Where(pair => pair.Key == "numero_rua_cliente").Select(pair => pair.Value).FirstOrDefault(),
                complement_end_cli: record.Where(pair => pair.Key == "complement_end_cli").Select(pair => pair.Value).FirstOrDefault(),
                bairro_cliente: record.Where(pair => pair.Key == "bairro_cliente").Select(pair => pair.Value).FirstOrDefault(),
                cep_cliente: record.Where(pair => pair.Key == "cep_cliente").Select(pair => pair.Value).FirstOrDefault(),
                cidade_cliente: record.Where(pair => pair.Key == "cidade_cliente").Select(pair => pair.Value).FirstOrDefault(),
                uf_cliente: record.Where(pair => pair.Key == "uf_cliente").Select(pair => pair.Value).FirstOrDefault(),
                pais: record.Where(pair => pair.Key == "pais").Select(pair => pair.Value).FirstOrDefault(),
                fone_cliente: record.Where(pair => pair.Key == "fone_cliente").Select(pair => pair.Value).FirstOrDefault(),
                email_cliente: record.Where(pair => pair.Key == "email_cliente").Select(pair => pair.Value).FirstOrDefault(),
                sexo: record.Where(pair => pair.Key == "sexo").Select(pair => pair.Value).FirstOrDefault(),
                data_cadastro: record.Where(pair => pair.Key == "data_cadastro").Select(pair => pair.Value).FirstOrDefault(),
                data_nascimento: record.Where(pair => pair.Key == "data_nascimento").Select(pair => pair.Value).FirstOrDefault(),
                cel_cliente: record.Where(pair => pair.Key == "cel_cliente").Select(pair => pair.Value).FirstOrDefault(),
                ativo: record.Where(pair => pair.Key == "ativo").Select(pair => pair.Value).FirstOrDefault(),
                dt_update: record.Where(pair => pair.Key == "dt_update").Select(pair => pair.Value).FirstOrDefault(),
                inscricao_estadual: record.Where(pair => pair.Key == "inscricao_estadual").Select(pair => pair.Value).FirstOrDefault(),
                incricao_municipal: record.Where(pair => pair.Key == "incricao_municipal").Select(pair => pair.Value).FirstOrDefault(),
                identidade_cliente: record.Where(pair => pair.Key == "identidade_cliente").Select(pair => pair.Value).FirstOrDefault(),
                cartao_fidelidade: record.Where(pair => pair.Key == "cartao_fidelidade").Select(pair => pair.Value).FirstOrDefault(),
                cod_ibge_municipio: record.Where(pair => pair.Key == "cod_ibge_municipio").Select(pair => pair.Value).FirstOrDefault(),
                classe_cliente: record.Where(pair => pair.Key == "classe_cliente").Select(pair => pair.Value).FirstOrDefault(),
                matricula_conveniado: record.Where(pair => pair.Key == "matricula_conveniado").Select(pair => pair.Value).FirstOrDefault(),
                tipo_cadastro: record.Where(pair => pair.Key == "tipo_cadastro").Select(pair => pair.Value).FirstOrDefault(),
                empresa_cadastro: record.Where(pair => pair.Key == "empresa_cadastro").Select(pair => pair.Value).FirstOrDefault(),
                id_estado_civil: record.Where(pair => pair.Key == "id_estado_civil").Select(pair => pair.Value).FirstOrDefault(),
                fax_cliente: record.Where(pair => pair.Key == "fax_cliente").Select(pair => pair.Value).FirstOrDefault(),
                site_cliente: record.Where(pair => pair.Key == "site_cliente").Select(pair => pair.Value).FirstOrDefault(),
                cliente_anonimo: record.Where(pair => pair.Key == "cliente_anonimo").Select(pair => pair.Value).FirstOrDefault(),
                limite_compras: record.Where(pair => pair.Key == "limite_compras").Select(pair => pair.Value).FirstOrDefault(),
                codigo_ws: record.Where(pair => pair.Key == "codigo_ws").Select(pair => pair.Value).FirstOrDefault(),
                limite_credito_compra: record.Where(pair => pair.Key == "limite_credito_compra").Select(pair => pair.Value).FirstOrDefault(),
                id_classe_fiscal: record.Where(pair => pair.Key == "id_classe_fiscal").Select(pair => pair.Value).FirstOrDefault(),
                obs: record.Where(pair => pair.Key == "obs").Select(pair => pair.Value).FirstOrDefault(),
                mae: record.Where(pair => pair.Key == "mae").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxClientesFornec CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxClientesFornec dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"doc_cliente: {record.Where(p => p.Key == "doc_cliente").Select(p => p.Value).FirstOrDefault()} | razao_cliente: {record.Where(p => p.Key == "razao_cliente").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxClientesFornec entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxClientesFornec entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxClientesFornec entity) =>
            $"doc_cliente: {entity.doc_cliente} | razao_cliente: {entity.razao_cliente}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxClientesFornec> records) =>
            await _linxClientesFornecRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.doc_cliente).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<LinxClientesFornec> records, LinxAPIParam jobParameter) =>
            _linxClientesFornecRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxClientesFornec?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxClientesFornec?>().ToList();

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters
                    .Replace("[0]", "0")
                    .Replace("[doc_cliente]", identificador)
                    .Replace("[data_inicial]", "2000-01-01")
                    .Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxClientesFornecRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> GetRecord(LinxAPIParam jobParameter)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            string _sql = _linxClientesFornecCommandHandler.CreateIntegrityLockQuery();
            var doc_clientes = await _coreRepository.GetRecords<string>(_sql);

            foreach (var identificador in doc_clientes)
            {
                var body = _linxMicrovixServiceBase.BuildBodyRequest(
                        parametersList: parameters
                            .Replace("[0]", "0")
                            .Replace("[doc_cliente]", identificador)
                            .Replace("[data_inicial]", "2000-01-01")
                            .Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
                        jobParameter: jobParameter,
                        cnpj_emp: jobParameter.DocMainCompany);

                string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

                if (xmls.Count() > 0)
                {
                    var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                    await _linxClientesFornecRepository.InsertRecord(record: listRecords.FirstOrDefault(), jobParameter: jobParameter);
                    await _linxClientesFornecRepository.InsertRecord(identifier: listRecords.FirstOrDefault().doc_cliente, is_present_in_erp: true);
                }
                else
                    await _linxClientesFornecRepository.InsertRecord(identifier: identificador, is_present_in_erp: false);
            }

            return true;
        }

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            string sqlTimestamp = _linxMicrovixCommandHandler.CreateGetLast7DaysMinTimestampQuery(jobParameter.schema, jobParameter.tableName, "dt_update");
            string? timestamp = await _coreRepository.GetRecord<string>(sqlTimestamp);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                        parametersList: parameters
                            .Replace("[0]", timestamp)
                            .Replace("[data_inicial]", $"{DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}")
                            .Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}")
                            .Replace("[dt_update_inicial]", $"{DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}")
                            .Replace("[dt_update_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
                        jobParameter: jobParameter,
                        cnpj_emp: jobParameter.DocMainCompany
                    );

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

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
