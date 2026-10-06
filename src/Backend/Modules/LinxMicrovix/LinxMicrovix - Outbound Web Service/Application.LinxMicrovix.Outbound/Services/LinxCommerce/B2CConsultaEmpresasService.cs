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
    public class B2CConsultaEmpresasService
        : B2CBaseSyncService<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaEmpresas, B2CConsultaEmpresas>,
          IB2CConsultaEmpresasService
    {
        private readonly IB2CConsultaEmpresasRepository _b2cConsultaEmpresasRepository;

        public B2CConsultaEmpresasService(
            IAPICall apiCall,
            ILoggerService logger,
            IIntegrationsCoreRepository coreRepository,
            ILinxMicrovixService linxMicrovixServiceBase,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaEmpresas> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IB2CConsultaEmpresasRepository b2cConsultaEmpresasRepository,
            IMemoryCache cache
        ) : base(apiCall, logger, linxMicrovixServiceBase, coreRepository, linxMicrovixCommandHandler, cache, validator)
        {
            _b2cConsultaEmpresasRepository = b2cConsultaEmpresasRepository;
        }

        protected override EnumJob JobName => EnumJob.B2CConsultaEmpresas;
        protected override string CacheKey => "B2CConsultaEmpresas:RecordKeys";

        protected override Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaEmpresas CreateDto(Dictionary<string?, string?> record) =>
            new(
                empresa: record.Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                nome_emp: record.Where(pair => pair.Key == "nome_emp").Select(pair => pair.Value).FirstOrDefault(),
                cnpj_emp: record.Where(pair => pair.Key == "cnpjEmp").Select(pair => pair.Value).FirstOrDefault(),
                end_unidade: record.Where(pair => pair.Key == "end_unidade").Select(pair => pair.Value).FirstOrDefault(),
                complemento_end_unidade: record.Where(pair => pair.Key == "complemento_end_unidade").Select(pair => pair.Value).FirstOrDefault(),
                nr_rua_unidade: record.Where(pair => pair.Key == "nr_rua_unidade").Select(pair => pair.Value).FirstOrDefault(),
                bairro_unidade: record.Where(pair => pair.Key == "bairro_unidade").Select(pair => pair.Value).FirstOrDefault(),
                cep_unidade: record.Where(pair => pair.Key == "cep_unidade").Select(pair => pair.Value).FirstOrDefault(),
                cidade_unidade: record.Where(pair => pair.Key == "cidade_unidade").Select(pair => pair.Value).FirstOrDefault(),
                uf_unidade: record.Where(pair => pair.Key == "uf_unidade").Select(pair => pair.Value).FirstOrDefault(),
                email_unidade: record.Where(pair => pair.Key == "email_unidade").Select(pair => pair.Value).FirstOrDefault(),
                timestamp: record.Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                data_criacao: record.Where(pair => pair.Key == "data_criacao").Select(pair => pair.Value).FirstOrDefault(),
                centro_distribuicao: record.Where(pair => pair.Key == "centro_distribuicao").Select(pair => pair.Value).FirstOrDefault(),
                portal: record.Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
            );

        protected override B2CConsultaEmpresas CreateEntity(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaEmpresas dto, string? xml) =>
            new(dto, xml);

        protected override string BuildErrorContext(Dictionary<string?, string?> record) =>
            $"empresa: {record.Where(p => p.Key == "empresa").Select(p => p.Value).FirstOrDefault()} | nome_emp: {record.Where(p => p.Key == "nome_emp").Select(p => p.Value).FirstOrDefault()}";

        protected override string? GetRecordKey(B2CConsultaEmpresas entity) => entity.recordKey;
        protected override string? GetRecordXml(B2CConsultaEmpresas entity) => entity.recordXml;

        protected override string BuildEntityLogContext(B2CConsultaEmpresas entity) =>
            $"empresa: {entity.empresa} | nome_emp: {entity.nome_emp}";

        protected override async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<B2CConsultaEmpresas> records) =>
            await _b2cConsultaEmpresasRepository.GetRegistersExists(
                jobParameter: jobParameter,
                registros: records.Select(x => x.cnpj_emp).ToList()
            );

        protected override void BulkInsertIntoTableRaw(List<B2CConsultaEmpresas> records, LinxAPIParam jobParameter) =>
            _b2cConsultaEmpresasRepository.BulkInsertIntoTableRaw(records: records, jobParameter: jobParameter);

        public List<B2CConsultaEmpresas?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records) =>
            DeserializeXMLToObjectCore(jobParameter, records).Cast<B2CConsultaEmpresas?>().ToList();

        public Task<bool> GetRecords(LinxAPIParam jobParameter) => GetRecordsCore(jobParameter);
    }
}
