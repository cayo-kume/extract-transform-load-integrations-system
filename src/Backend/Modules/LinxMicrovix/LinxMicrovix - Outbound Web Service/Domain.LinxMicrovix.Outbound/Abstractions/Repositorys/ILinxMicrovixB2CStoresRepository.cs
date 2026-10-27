using Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Stores;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixB2CStoresRepository
    {
        public Task<bool> BulkInsertIntoB2CConsultaEmpresas(List<B2CConsultaEmpresas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaCNPJsChave(List<B2CConsultaCNPJsChave> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaVendedores(List<B2CConsultaVendedores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaTransportadores(List<B2CConsultaTransportadores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaFormasPagamento(List<B2CConsultaFormasPagamento> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaPlanos(List<B2CConsultaPlanos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaPlanosParcelas(List<B2CConsultaPlanosParcelas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaTiposCobrancaFrete(List<B2CConsultaTiposCobrancaFrete> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaTipoEncomenda(List<B2CConsultaTipoEncomenda> data, Guid? parentExecutionGUID);
    }
}
