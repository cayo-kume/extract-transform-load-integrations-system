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
    public class LinxProdutosDepositosService
        : LinxMicrovixBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosDepositos, LinxProdutosDepositos>,
          ILinxProdutosDepositosService
    {
        private readonly ILinxProdutosDepositosRepository _linxProdutosDepositosRepository;

        public LinxProdutosDepositosService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            ILinxProdutosDepositosRepository linxProdutosDepositosRepository,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosDepositos> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _linxProdutosDepositosRepository = linxProdutosDepositosRepository;
        }

        protected override EnumJob JobName => EnumJob.LinxProdutosDepositos;
        protected override string CacheKey => "LinxProdutosDepositos:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosDepositos CreateDto(Dictionary<string?, string?> record) =>
            new(
                cod_deposito: record.Where(pair => pair.Key == "cod_deposito").Select(pair => pair.Value).FirstOrDefault(),
                nome_deposito: record.Where(pair => pair.Key == "nome_deposito").Select(pair => pair.Value).FirstOrDefault(),
                disponivel: record.Where(pair => pair.Key == "disponivel").Select(pair => pair.Value).FirstOrDefault(),
                disponivel_transferencia: record.Where(pair => pair.Key == "disponivel_transferencia").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override LinxProdutosDepositos CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosDepositos dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"cod_deposito: {record.Where(p => p.Key == "cod_deposito").Select(p => p.Value).FirstOrDefault()} | nome_deposito: {record.Where(p => p.Key == "nome_deposito").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(LinxProdutosDepositos entity) => entity.recordKey;
        protected override string? GetRecordXml(LinxProdutosDepositos entity) => entity.recordXml;

        protected override string BuildEntityLogContext(LinxProdutosDepositos entity) =>
            $"cod_deposito: {entity.cod_deposito} | nome_deposito: {entity.nome_deposito}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<LinxProdutosDepositos> records) =>
            await _linxProdutosDepositosRepository.GetRegistersExists();

        protected override void BulkInsertIntoTableRaw(List<LinxProdutosDepositos> records, LinxAPIParam jobParameter) =>
            _linxProdutosDepositosRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<LinxProdutosDepositos?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<LinxProdutosDepositos?>().ToList();

        public Task<bool> GetRecords(LinxAPIParam jobParameter) => GetRecordsCore(jobParameter);

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters.Replace("[0]", "0").Replace("[cod_deposito]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: jobParameter.DocMainCompany);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxProdutosDepositosRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }
    }
}
