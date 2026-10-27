using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixFiscalDocumentsRepository
    {
        public Task<bool> BulkInsertIntoLinxXMLDocumentos(List<LinxXMLDocumentos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxNFeEvento(List<LinxNFeEvento> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxNfse(List<LinxNfse> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxEntradaNFe(List<LinxEntradaNFe> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxEntradaNFeProdutos(List<LinxEntradaNFeProdutos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxConhecimentoFretes(List<LinxConhecimentoFretes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxFreteTipos(List<LinxFreteTipos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxReducoesZ(List<LinxReducoesZ> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxECF(List<LinxECF> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxECFFormasPagamento(List<LinxECFFormasPagamento> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxNfceEstacao(List<LinxNfceEstacao> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxSeries(List<LinxSeries> data, Guid? parentExecutionGUID);
    }
}
