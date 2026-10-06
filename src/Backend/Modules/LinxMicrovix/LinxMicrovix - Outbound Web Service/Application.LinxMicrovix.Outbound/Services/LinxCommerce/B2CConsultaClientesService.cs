using Application.Core.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxCommerce;
using Domain.Core.Abstractions.Repositorys;
using Domain.Core.Enums;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace Application.LinxMicrovix.Outbound.WebService.Services
{
    public class B2CConsultaClientesService
        : B2CBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaClientes, B2CConsultaClientes>,
          IB2CConsultaClientesService
    {
        private readonly IB2CConsultaClientesRepository _b2cConsultaClientesRepository;
        private readonly IB2CConsultaClientesCommandHandler _b2CConsultaClientesCommandHandler;

        public B2CConsultaClientesService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            IB2CConsultaClientesRepository b2cConsultaClientesRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaClientes> validator,
            IB2CConsultaClientesCommandHandler b2CConsultaClientesCommandHandler,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _b2cConsultaClientesRepository = b2cConsultaClientesRepository;
            _b2CConsultaClientesCommandHandler = b2CConsultaClientesCommandHandler;
        }

        protected override EnumJob JobName => EnumJob.B2CConsultaClientes;
        protected override string CacheKey => "B2CConsultaClientes:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaClientes CreateDto(Dictionary<string?, string?> record) =>
            new(
                cod_cliente_b2c: record.Where(pair => pair.Key == "cod_cliente_b2c").Select(pair => pair.Value).FirstOrDefault(),
                cod_cliente_erp: record.Where(pair => pair.Key == "cod_cliente_erp").Select(pair => pair.Value).FirstOrDefault(),
                doc_cliente: record.Where(pair => pair.Key == "doc_cliente").Select(pair => pair.Value).FirstOrDefault(),
                nm_cliente: record.Where(pair => pair.Key == "nm_cliente").Select(pair => pair.Value).FirstOrDefault(),
                nm_mae: record.Where(pair => pair.Key == "nm_mae").Select(pair => pair.Value).FirstOrDefault(),
                nm_pai: record.Where(pair => pair.Key == "nm_pai").Select(pair => pair.Value).FirstOrDefault(),
                nm_conjuge: record.Where(pair => pair.Key == "nm_conjuge").Select(pair => pair.Value).FirstOrDefault(),
                dt_cadastro: record.Where(pair => pair.Key == "dt_cadastro").Select(pair => pair.Value).FirstOrDefault(),
                dt_nasc_cliente: record.Where(pair => pair.Key == "dt_nasc_cliente").Select(pair => pair.Value).FirstOrDefault(),
                end_cliente: record.Where(pair => pair.Key == "end_cliente").Select(pair => pair.Value).FirstOrDefault(),
                complemento_end_cliente: record.Where(pair => pair.Key == "complemento_end_cliente").Select(pair => pair.Value).FirstOrDefault(),
                nr_rua_cliente: record.Where(pair => pair.Key == "nr_rua_cliente").Select(pair => pair.Value).FirstOrDefault(),
                bairro_cliente: record.Where(pair => pair.Key == "bairro_cliente").Select(pair => pair.Value).FirstOrDefault(),
                cep_cliente: record.Where(pair => pair.Key == "cep_cliente").Select(pair => pair.Value).FirstOrDefault(),
                cidade_cliente: record.Where(pair => pair.Key == "cidade_cliente").Select(pair => pair.Value).FirstOrDefault(),
                uf_cliente: record.Where(pair => pair.Key == "uf_cliente").Select(pair => pair.Value).FirstOrDefault(),
                fone_cliente: record.Where(pair => pair.Key == "fone_cliente").Select(pair => pair.Value).FirstOrDefault(),
                fone_comercial: record.Where(pair => pair.Key == "fone_comercial").Select(pair => pair.Value).FirstOrDefault(),
                cel_cliente: record.Where(pair => pair.Key == "cel_cliente").Select(pair => pair.Value).FirstOrDefault(),
                email_cliente: record.Where(pair => pair.Key == "email_cliente").Select(pair => pair.Value).FirstOrDefault(),
                rg_cliente: record.Where(pair => pair.Key == "rg_cliente").Select(pair => pair.Value).FirstOrDefault(),
                rg_orgao_emissor: record.Where(pair => pair.Key == "rg_orgao_emissor").Select(pair => pair.Value).FirstOrDefault(),
                estado_civil_cliente: record.Where(pair => pair.Key == "estado_civil_cliente").Select(pair => pair.Value).FirstOrDefault(),
                empresa_cliente: record.Where(pair => pair.Key == "empresa_cliente").Select(pair => pair.Value).FirstOrDefault(),
                cargo_cliente: record.Where(pair => pair.Key == "cargo_cliente").Select(pair => pair.Value).FirstOrDefault(),
                sexo_cliente: record.Where(pair => pair.Key == "sexo_cliente").Select(pair => pair.Value).FirstOrDefault(),
                dt_update: record.Where(pair => pair.Key == "dt_update").Select(pair => pair.Value).FirstOrDefault(),
                ativo: record.Where(pair => pair.Key == "ativo").Select(pair => pair.Value).FirstOrDefault(),
                receber_email: record.Where(pair => pair.Key == "receber_email").Select(pair => pair.Value).FirstOrDefault(),
                dt_expedicao_rg: record.Where(pair => pair.Key == "dt_expedicao_rg").Select(pair => pair.Value).FirstOrDefault(),
                naturalidade: record.Where(pair => pair.Key == "naturalidade").Select(pair => pair.Value).FirstOrDefault(),
                tempo_residencia: record.Where(pair => pair.Key == "tempo_residencia").Select(pair => pair.Value).FirstOrDefault(),
                renda: record.Where(pair => pair.Key == "renda").Select(pair => pair.Value).FirstOrDefault(),
                numero_compl_rua_cliente: record.Where(pair => pair.Key == "numero_compl_rua_cliente").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                tipo_pessoa: record.Where(pair => pair.Key == "tipo_pessoa").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault(),
                aceita_programa_fidelidade: record.Where(pair => pair.Key == "aceita_programa_fidelidade").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override B2CConsultaClientes CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaClientes dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"cod_cliente_b2c: {record.Where(p => p.Key == "cod_cliente_b2c").Select(p => p.Value).FirstOrDefault()} | nm_cliente: {record.Where(p => p.Key == "nm_cliente").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(B2CConsultaClientes entity) => entity.recordKey;
        protected override string? GetRecordXml(B2CConsultaClientes entity) => entity.recordXml;

        protected override string BuildEntityLogContext(B2CConsultaClientes entity) =>
            $"cod_cliente_b2c: {entity.cod_cliente_b2c} | nm_cliente: {entity.nm_cliente}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<B2CConsultaClientes> records) =>
            await _b2cConsultaClientesRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.doc_cliente).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<B2CConsultaClientes> records, LinxAPIParam jobParameter) =>
            _b2cConsultaClientesRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<B2CConsultaClientes?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<B2CConsultaClientes?>().ToList();

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            string sqlTimestamp = _linxMicrovixCommandHandler.CreateGetLast7DaysMinTimestampQuery(jobParameter.schema, jobParameter.tableName, "DT_UPDATE");
            string? timestamp = await _coreRepository.GetRecord<string>(sqlTimestamp);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", timestamp),
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

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[doc_cliente]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _b2cConsultaClientesRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> IntegrityLockB2CConsultaClientesRegisters(LinxAPIParam jobParameter)
        {
            try
            {
                var _listSomenteNovos = new List<B2CConsultaClientes>();

                string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
                string? parameters = await _coreRepository.GetRecord<string>(sql);

                string integritySql = _b2CConsultaClientesCommandHandler.CreateIntegrityLockQuery();
                var _listRegistrosConsultados = await _coreRepository.GetRecords<IntegrityLockTablesRegister>(integritySql);

                foreach (var cliente in _listRegistrosConsultados)
                {
                    var body = _linxMicrovixServiceBase.BuildBodyRequest(
                                    parametersList: parameters.Replace("[0]", "0").Replace("[doc_cliente]", cliente.identifier).Replace("[data_inicial]", "2000-01-01").Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
                                    jobParameter: jobParameter,
                                    cnpj_emp: jobParameter.DocMainCompany
                                );

                    string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                    var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

                    if (xmls.Count() > 0)
                    {
                        var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);
                        _listSomenteNovos.AddRange(listRecords);
                        cliente.is_present_in_erp = true;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
