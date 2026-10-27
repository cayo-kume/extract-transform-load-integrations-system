using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixTaxesRepository
    {
        public Task<bool> BulkInsertIntoLinxNaturezaOperacao(List<LinxNaturezaOperacao> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCfopFiscal(List<LinxCfopFiscal> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCstIcmsFiscal(List<LinxCstIcmsFiscal> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCstIpiFiscal(List<LinxCstIpiFiscal> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCstPisFiscal(List<LinxCstPisFiscal> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCstCofinsFiscal(List<LinxCstCofinsFiscal> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCsosnFiscal(List<LinxCsosnFiscal> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxClasseFiscal(List<LinxClasseFiscal> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCest(List<LinxCest> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxNcm(List<LinxNcm> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxEnquadramentoIpi(List<LinxEnquadramentoIpi> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMotivosDesoneracaoIcms(List<LinxMotivosDesoneracaoIcms> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxSpedTipoItem(List<LinxSpedTipoItem> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxSpedTipoBaseCredito(List<LinxSpedTipoBaseCredito> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxRotinaOrigem(List<LinxRotinaOrigem> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxNfeIbgeMunicipio(List<LinxNfeIbgeMunicipio> data, Guid? parentExecutionGUID);
    }
}
