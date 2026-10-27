using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ProductAttributes;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixProductAttributesRepository
    {
        public Task<bool> BulkInsertIntoLinxSetores(List<LinxSetores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxLinhas(List<LinxLinhas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMarcas(List<LinxMarcas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxColecoes(List<LinxColecoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxTamanhos(List<LinxTamanhos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCores(List<LinxCores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxEspessuras(List<LinxEspessuras> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxUnidades(List<LinxUnidades> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxClassificacoes(List<LinxClassificacoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxSubClasses(List<LinxSubClasses> data, Guid? parentExecutionGUID);
    }
}
