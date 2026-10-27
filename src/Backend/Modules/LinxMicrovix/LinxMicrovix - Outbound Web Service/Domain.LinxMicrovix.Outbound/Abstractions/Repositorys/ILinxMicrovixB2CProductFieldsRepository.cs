using Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixB2CProductFieldsRepository
    {
        public Task<bool> BulkInsertIntoB2CConsultaProdutosCamposAdicionaisDetalhes(List<B2CConsultaProdutosCamposAdicionaisDetalhes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosCamposAdicionaisNomes(List<B2CConsultaProdutosCamposAdicionaisNomes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosCamposAdicionaisValores(List<B2CConsultaProdutosCamposAdicionaisValores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionais(List<B2CConsultaB2CProdutosCamposAdicionais> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisDados(List<B2CConsultaB2CProdutosCamposAdicionaisDados> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisMedia(List<B2CConsultaB2CProdutosCamposAdicionaisMedia> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisMediaDados(List<B2CConsultaB2CProdutosCamposAdicionaisMediaDados> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisOpcoes(List<B2CConsultaB2CProdutosCamposAdicionaisOpcoes> data, Guid? parentExecutionGUID);
    }
}
