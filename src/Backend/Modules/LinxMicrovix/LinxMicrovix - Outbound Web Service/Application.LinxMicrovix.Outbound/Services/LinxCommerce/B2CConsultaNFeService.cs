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
    public class B2CConsultaNFeService
        : B2CBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaNFe, B2CConsultaNFe>,
          IB2CConsultaNFeService
    {
        private readonly IB2CConsultaNFeRepository _b2cConsultaNFeRepository;
        private readonly IB2CConsultaNFeCommandHandler _b2cConsultaNFeCommandHandler;

        public B2CConsultaNFeService(
            IAPICall apiCall,
            ILoggerService logger,
            IIntegrationsCoreRepository coreRepository,
            ILinxMicrovixService linxMicrovixServiceBase,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaNFe> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IB2CConsultaNFeCommandHandler b2cConsultaNFeCommandHandler,
            IB2CConsultaNFeRepository b2cConsultaNFeRepository,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _b2cConsultaNFeRepository = b2cConsultaNFeRepository;
            _b2cConsultaNFeCommandHandler = b2cConsultaNFeCommandHandler;
        }

        protected override EnumJob JobName => EnumJob.B2CConsultaNFe;
        protected override string CacheKey => "B2CConsultaNFe:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaNFe CreateDto(Dictionary<string?, string?> record) =>
            new(
                id_nfe: record.Where(pair => pair.Key == "id_nfe").Select(pair => pair.Value).FirstOrDefault(),
                id_pedido: record.Where(pair => pair.Key == "id_pedido").Select(pair => pair.Value).FirstOrDefault(),
                documento: record.Where(pair => pair.Key == "documento").Select(pair => pair.Value).FirstOrDefault(),
                data_emissao: record.Where(pair => pair.Key == "data_emissao").Select(pair => pair.Value).FirstOrDefault(),
                chave_nfe: record.Where(pair => pair.Key == "chave_nfe").Select(pair => pair.Value).FirstOrDefault(),
                situacao: record.Where(pair => pair.Key == "situacao").Select(pair => pair.Value).FirstOrDefault(),
                xml: record.Where(pair => pair.Key == "xml").Select(pair => pair.Value).FirstOrDefault(),
                excluido: record.Where(pair => pair.Key == "excluido").Select(pair => pair.Value).FirstOrDefault(),
                identificador_microvix: record.Where(pair => pair.Key == "identificador_microvix").Select(pair => pair.Value).FirstOrDefault(),
                dt_insert: record.Where(pair => pair.Key == "dt_insert").Select(pair => pair.Value).FirstOrDefault(),
                valor_nota: record.Where(pair => pair.Key == "valor_nota").Select(pair => pair.Value).FirstOrDefault(),
                serie: record.Where(pair => pair.Key == "serie").Select(pair => pair.Value).FirstOrDefault(),
                frete: record.Where(pair => pair.Key == "frete").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault(),
                nProt: record.Where(pair => pair.Key == "nProt").Select(pair => pair.Value).FirstOrDefault(),
                codigo_modelo_nf: record.Where(pair => pair.Key == "codigo_modelo_nf").Select(pair => pair.Value).FirstOrDefault(),
                justificativa: record.Where(pair => pair.Key == "justificativa").Select(pair => pair.Value).FirstOrDefault(),
                tpAmb: record.Where(pair => pair.Key == "tpAmb").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override B2CConsultaNFe CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaNFe dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"id_nfe: {record.Where(p => p.Key == "id_nfe").Select(p => p.Value).FirstOrDefault()} | chave_nfe: {record.Where(p => p.Key == "chave_nfe").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(B2CConsultaNFe entity) => entity.recordKey;
        protected override string? GetRecordXml(B2CConsultaNFe entity) => entity.recordXml;

        protected override string BuildEntityLogContext(B2CConsultaNFe entity) =>
            $"id_nfe: {entity.id_nfe} | chave_nfe: {entity.chave_nfe}";

        protected override string BuildRequestBodyParameters(string parameters, LinxAPIParam jobParameter) =>
            parameters
                .Replace("[0]", "0")
                .Replace("[data_inicial]", $"{DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}")
                .Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}");

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<B2CConsultaNFe> records) =>
            await _b2cConsultaNFeRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.id_nfe).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<B2CConsultaNFe> records, LinxAPIParam jobParameter) =>
            _b2cConsultaNFeRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<B2CConsultaNFe?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<B2CConsultaNFe?>().ToList();

        public Task<bool> GetRecords(LinxAPIParam jobParameter) => GetRecordsCore(jobParameter);

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[id_pedido]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _b2cConsultaNFeRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> IntegrityLockRegisters(LinxAPIParam jobParameter)
        {
            try
            {
                var _listSomenteNovos = new List<B2CConsultaNFe>();

                string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
                string? parameters = await _coreRepository.GetRecord<string>(sql);

                string integritySql = _b2cConsultaNFeCommandHandler.CreateIntegrityLockQuery();
                var _listRegistrosConsultados = await _coreRepository.GetRecords<IntegrityLockTablesRegister>(integritySql);

                foreach (var pedido in _listRegistrosConsultados)
                {
                    var body = _linxMicrovixServiceBase.BuildBodyRequest(
                                    parametersList: parameters.Replace("[0]", "0").Replace("[id_pedido]", pedido.identifier).Replace("[data_inicial]", "2000-01-01").Replace("[data_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
                                    jobParameter: jobParameter,
                                    cnpj_emp: jobParameter.DocMainCompany
                                );

                    string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                    var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

                    if (xmls.Count() > 0)
                    {
                        var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);
                        _listSomenteNovos.AddRange(listRecords);
                        pedido.is_present_in_erp = true;
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
