using Application.Core.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
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
    public class B2CConsultaNFeSituacaoService
        : B2CBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaNFeSituacao, B2CConsultaNFeSituacao>,
          IB2CConsultaNFeSituacaoService
    {
        private readonly IB2CConsultaNFeSituacaoRepository _b2cConsultaNFeSituacaoRepository;

        public B2CConsultaNFeSituacaoService(
            IAPICall apiCall,
            ILoggerService logger,
            IIntegrationsCoreRepository coreRepository,
            ILinxMicrovixService linxMicrovixServiceBase,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaNFeSituacao> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IB2CConsultaNFeSituacaoRepository b2cConsultaNFeSituacaoRepository,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _b2cConsultaNFeSituacaoRepository = b2cConsultaNFeSituacaoRepository;
        }

        protected override EnumJob JobName => EnumJob.B2CConsultaNFeSituacao;
        protected override string CacheKey => "B2CConsultaNFeSituacao:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaNFeSituacao CreateDto(Dictionary<string?, string?> record) =>
            new(
                id_nfe_situacao: record.Where(pair => pair.Key == "id_nfe_situacao").Select(pair => pair.Value).FirstOrDefault(),
                descricao: record.Where(pair => pair.Key == "descricao").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override B2CConsultaNFeSituacao CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaNFeSituacao dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"id_nfe_situacao: {record.Where(p => p.Key == "id_nfe_situacao").Select(p => p.Value).FirstOrDefault()} | descricao: {record.Where(p => p.Key == "descricao").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(B2CConsultaNFeSituacao entity) => entity.recordKey;
        protected override string? GetRecordXml(B2CConsultaNFeSituacao entity) => entity.recordXml;

        protected override string BuildEntityLogContext(B2CConsultaNFeSituacao entity) =>
            $"id_nfe_situacao: {entity.id_nfe_situacao} | descricao: {entity.descricao}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<B2CConsultaNFeSituacao> records) =>
            await _b2cConsultaNFeSituacaoRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.id_nfe_situacao).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<B2CConsultaNFeSituacao> records, LinxAPIParam jobParameter) =>
            _b2cConsultaNFeSituacaoRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<B2CConsultaNFeSituacao?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<B2CConsultaNFeSituacao?>().ToList();

        public Task<bool> GetRecords(LinxAPIParam jobParameter) => GetRecordsCore(jobParameter);
    }
}
