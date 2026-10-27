using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.TaxSettings;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixTaxSettingsRepository
    {
        public Task<bool> BulkInsertIntoLinxConfiguracoesTributarias(List<LinxConfiguracoesTributarias> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxConfiguracoesTributariasDetalhes(List<LinxConfiguracoesTributariasDetalhes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxConfiguracoesTributariasDetalhesSimplificado(List<LinxConfiguracoesTributariasDetalhesSimplificado> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxConfiguracoesTributariasEmpresas(List<LinxConfiguracoesTributariasEmpresas> data, Guid? parentExecutionGUID);
    }
}
