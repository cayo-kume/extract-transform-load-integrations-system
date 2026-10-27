using Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductAttributes;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixB2CProductAttributesRepository
    {
        public Task<bool> BulkInsertIntoB2CConsultaClassificacao(List<B2CConsultaClassificacao> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaColecoes(List<B2CConsultaColecoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaEspessuras(List<B2CConsultaEspessuras> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaGrade1(List<B2CConsultaGrade1> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaGrade2(List<B2CConsultaGrade2> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaLinhas(List<B2CConsultaLinhas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaMarcas(List<B2CConsultaMarcas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaSetores(List<B2CConsultaSetores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaUnidade(List<B2CConsultaUnidade> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaFornecedores(List<B2CConsultaFornecedores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaFlags(List<B2CConsultaFlags> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaTags(List<B2CConsultaTags> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaPalavrasChavePesquisa(List<B2CConsultaPalavrasChavePesquisa> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaLegendasCadastrosAuxiliares(List<B2CConsultaLegendasCadastrosAuxiliares> data, Guid? parentExecutionGUID);
    }
}
