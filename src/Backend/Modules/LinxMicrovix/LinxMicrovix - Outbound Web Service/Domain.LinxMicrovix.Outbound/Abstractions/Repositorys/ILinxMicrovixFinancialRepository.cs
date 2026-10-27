using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixFinancialRepository
    {
        public Task<bool> BulkInsertIntoLinxFaturas(List<LinxFaturas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxFaturasOrigem(List<LinxFaturasOrigem> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPortadores(List<LinxPortadores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxAntecipacoesFinanceiras(List<LinxAntecipacoesFinanceiras> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxAntecipacoesFinanceirasPlanos(List<LinxAntecipacoesFinanceirasPlanos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxDesfazimentosAntecipacoes(List<LinxDesfazimentosAntecipacoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxDesfazimentosAntecipacoesDocumento(List<LinxDesfazimentosAntecipacoesDocumento> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxDesfazimentosAntecipacoesItens(List<LinxDesfazimentosAntecipacoesItens> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCategoriasFinanceiras(List<LinxCategoriasFinanceiras> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxCentroCusto(List<LinxCentroCusto> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPlanoContas(List<LinxPlanoContas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxLancContabil(List<LinxLancContabil> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxHistoricosContabeis(List<LinxHistoricosContabeis> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxExtratoBancarioLancamentos(List<LinxExtratoBancarioLancamentos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxExtratoBancarioConciliacoes(List<LinxExtratoBancarioConciliacoes> data, Guid? parentExecutionGUID);
    }
}
