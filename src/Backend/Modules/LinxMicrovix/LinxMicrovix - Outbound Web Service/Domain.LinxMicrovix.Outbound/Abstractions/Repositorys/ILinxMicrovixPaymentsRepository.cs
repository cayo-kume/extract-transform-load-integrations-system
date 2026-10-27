using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Payments;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixPaymentsRepository
    {
        public Task<bool> BulkInsertIntoLinxMovimentoCartoes(List<LinxMovimentoCartoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoPlanos(List<LinxMovimentoPlanos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPlanos(List<LinxPlanos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPlanosParcelas(List<LinxPlanosParcelas> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxPlanosBandeiras(List<LinxPlanosBandeiras> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoGiftCard(List<LinxMovimentoGiftCard> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxValesComprasEnviadosAPI(List<LinxValesComprasEnviadosAPI> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxSangriaSuprimentos(List<LinxSangriaSuprimentos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxFechamentoCaixa(List<LinxFechamentoCaixa> data, Guid? parentExecutionGUID);
    }
}
