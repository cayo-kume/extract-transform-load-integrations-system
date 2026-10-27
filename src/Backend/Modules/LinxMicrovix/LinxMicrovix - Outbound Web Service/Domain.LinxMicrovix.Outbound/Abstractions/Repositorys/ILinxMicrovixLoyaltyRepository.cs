using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Loyalty;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixLoyaltyRepository
    {
        public Task<bool> BulkInsertIntoLinxFidelidade(List<LinxFidelidade> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoReshop(List<LinxMovimentoReshop> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMovimentoReshopItens(List<LinxMovimentoReshopItens> data, Guid? parentExecutionGUID);
    }
}
