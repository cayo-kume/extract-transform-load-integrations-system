using Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.StocksAndPrices;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixB2CStocksAndPricesRepository
    {
        public Task<bool> BulkInsertIntoB2CConsultaProdutosCustos(List<B2CConsultaProdutosCustos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosDepositos(List<B2CConsultaProdutosDepositos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosDetalhes(List<B2CConsultaProdutosDetalhes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosDetalhesDepositos(List<B2CConsultaProdutosDetalhesDepositos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosPromocao(List<B2CConsultaProdutosPromocao> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosTabelas(List<B2CConsultaProdutosTabelas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosTabelasPrecos(List<B2CConsultaProdutosTabelasPrecos> data, Guid? parentExecutionGUID);
    }
}
