using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stocks;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixStocksRepository
    {
        public Task<bool> BulkInsertIntoLinxProdutosDetalhes(List<LinxProdutosDetalhes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosDetalhesSimplificado(List<LinxProdutosDetalhesSimplificado> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosDetalhesDepositos(List<LinxProdutosDetalhesDepositos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosDepositos(List<LinxProdutosDepositos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosInventario(List<LinxProdutosInventario> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxListagemBalanco(List<LinxListagemBalanco> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxBalancos(List<LinxBalancos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxBalancosArquivos(List<LinxBalancosArquivos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxBalancosSecoes(List<LinxBalancosSecoes> data, Guid? parentExecutionGUID);
    }
}
