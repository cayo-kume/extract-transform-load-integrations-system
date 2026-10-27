using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixMovementsRepository
    {
        public Task<bool> BulkInsertIntoLinxMovimento(List<LinxMovimento> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoPrincipal(List<LinxMovimentoPrincipal> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoExtensao(List<LinxMovimentoExtensao> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoCamposAdicionais(List<LinxMovimentoCamposAdicionais> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoObservacoes(List<LinxMovimentoObservacoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoObservacaoCF(List<LinxMovimentoObservacaoCF> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoIndicacoes(List<LinxMovimentoIndicacoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoSerial(List<LinxMovimentoSerial> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxSerialVenda(List<LinxSerialVenda> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoVendaConjugada(List<LinxMovimentoVendaConjugada> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPerguntaVenda(List<LinxPerguntaVenda> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxRespostaVenda(List<LinxRespostaVenda> data, Guid? parentExecutionGUID);
    }
}
