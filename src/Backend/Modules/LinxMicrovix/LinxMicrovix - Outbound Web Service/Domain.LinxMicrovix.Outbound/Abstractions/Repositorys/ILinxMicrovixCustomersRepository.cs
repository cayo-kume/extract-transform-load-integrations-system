using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixCustomersRepository
    {
        public Task<bool> BulkInsertIntoLinxClientesFornec(List<LinxClientesFornec> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxClientesFornecCamposAdicionais(List<LinxClientesFornecCamposAdicionais> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxClientesFornecClasses(List<LinxClientesFornecClasses> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxClientesFornecContatos(List<LinxClientesFornecContatos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxClientesFornecContatosParentesco(List<LinxClientesFornecContatosParentesco> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxClientesFornecCreditoAvulso(List<LinxClientesFornecCreditoAvulso> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxClientesFornecVendedores(List<LinxClientesFornecVendedores> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxClientesEnderecosEntrega(List<LinxClientesEnderecosEntrega> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxClientesRede(List<LinxClientesRede> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoLinxRamosAtividade(List<LinxRamosAtividade> data, Guid? parentExecutionGUID);
    }
}
