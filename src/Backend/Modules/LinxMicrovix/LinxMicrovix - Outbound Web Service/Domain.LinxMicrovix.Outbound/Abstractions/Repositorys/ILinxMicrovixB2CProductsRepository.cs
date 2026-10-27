using Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Products;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixB2CProductsRepository
    {
        public Task<bool> BulkInsertIntoB2CConsultaProdutos(List<B2CConsultaProdutos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosAssociados(List<B2CConsultaProdutosAssociados> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosCampanhas(List<B2CConsultaProdutosCampanhas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosCodebar(List<B2CConsultaProdutosCodebar> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosDimensoes(List<B2CConsultaProdutosDimensoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosInformacoes(List<B2CConsultaProdutosInformacoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosStatus(List<B2CConsultaProdutosStatus> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosFlags(List<B2CConsultaProdutosFlags> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosTags(List<B2CConsultaProdutosTags> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosPalavrasChavePesquisa(List<B2CConsultaProdutosPalavrasChavePesquisa> data, Guid? parentExecutionGUID);
    }
}
