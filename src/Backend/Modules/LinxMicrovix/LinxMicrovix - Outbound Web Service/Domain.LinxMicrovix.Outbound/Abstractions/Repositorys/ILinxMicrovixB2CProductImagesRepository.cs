using Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductImages;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixB2CProductImagesRepository
    {
        public Task<bool> BulkInsertIntoB2CConsultaProdutosImagens(List<B2CConsultaProdutosImagens> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaProdutosImagensURL(List<B2CConsultaProdutosImagensURL> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaImagens(List<B2CConsultaImagens> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaImagensHD(List<B2CConsultaImagensHD> data, Guid? parentExecutionGUID);
    }
}
