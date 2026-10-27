using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixPricesRepository
    {
        public Task<bool> BulkInsertIntoLinxProdutosTabelas(List<LinxProdutosTabelas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosTabelasPrecos(List<LinxProdutosTabelasPrecos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxProdutosPromocoes(List<LinxProdutosPromocoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxAcoesPromocionais(List<LinxAcoesPromocionais> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxAcoesPromocionaisCombinacaoProdutosItens(List<LinxAcoesPromocionaisCombinacaoProdutosItens> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxAcoesPromocionaisProdutosCortesia(List<LinxAcoesPromocionaisProdutosCortesia> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoAcoesPromocionais(List<LinxMovimentoAcoesPromocionais> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCupomDesconto(List<LinxCupomDesconto> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCupomDescontoVendas(List<LinxCupomDescontoVendas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMotivoDesconto(List<LinxMotivoDesconto> data, Guid? parentExecutionGUID);
    }
}
