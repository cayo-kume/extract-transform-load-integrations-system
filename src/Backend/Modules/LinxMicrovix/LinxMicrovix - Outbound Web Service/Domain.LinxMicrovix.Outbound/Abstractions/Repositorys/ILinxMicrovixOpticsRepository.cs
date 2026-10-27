using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Optics;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixOpticsRepository
    {
        public Task<bool> BulkInsertIntoLinxProdutosOpticosTipo(List<LinxProdutosOpticosTipo> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosOpticosTipoAro(List<LinxProdutosOpticosTipoAro> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosOpticosFormatoAro(List<LinxProdutosOpticosFormatoAro> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxDadosOpticosDav(List<LinxDadosOpticosDav> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxOticoPrismaDescricao(List<LinxOticoPrismaDescricao> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxOticoReceitas(List<LinxOticoReceitas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPosicaoOsRamoOptico(List<LinxPosicaoOsRamoOptico> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxOrdensServicoPosicaoOsRamoOpticoHistorico(List<LinxOrdensServicoPosicaoOsRamoOpticoHistorico> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxOrcamentoComponenteFormula(List<LinxOrcamentoComponenteFormula> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxComponenteFormulaDav(List<LinxComponenteFormulaDav> data, Guid? parentExecutionGUID);
    }
}
