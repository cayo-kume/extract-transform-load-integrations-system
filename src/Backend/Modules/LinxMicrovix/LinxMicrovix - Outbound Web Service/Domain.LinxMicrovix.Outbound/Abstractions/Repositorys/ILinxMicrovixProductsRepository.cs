using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixProductsRepository
    {
        public Task<bool> BulkInsertIntoLinxProdutos(List<LinxProdutos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosAssociados(List<LinxProdutosAssociados> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosCamposAdicionais(List<LinxProdutosCamposAdicionais> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosCodBar(List<LinxProdutosCodBar> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosFornec(List<LinxProdutosFornec> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosInformacoes(List<LinxProdutosInformacoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosImagensURL(List<LinxProdutosImagensURL> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosLotes(List<LinxProdutosLotes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxLotesProdutos(List<LinxLotesProdutos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosSerial(List<LinxProdutosSerial> data, Guid? parentExecutionGUID);
    }
}
