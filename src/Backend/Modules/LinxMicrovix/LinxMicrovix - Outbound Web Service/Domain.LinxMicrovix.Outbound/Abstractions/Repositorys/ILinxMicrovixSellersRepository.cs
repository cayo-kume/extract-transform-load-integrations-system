using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Sellers;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixSellersRepository
    {
        public Task<bool> BulkInsertIntoLinxVendedores(List<LinxVendedores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxVendedoresFerias(List<LinxVendedoresFerias> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMetasVendedores(List<LinxMetasVendedores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxMetasVendedoresDia(List<LinxMetasVendedoresDia> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxComissoesVendedores(List<LinxComissoesVendedores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxListaDaVez(List<LinxListaDaVez> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxFuncionariosFuncoes(List<LinxFuncionariosFuncoes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxRegistroProfissional(List<LinxRegistroProfissional> data, Guid? parentExecutionGUID);
    }
}
