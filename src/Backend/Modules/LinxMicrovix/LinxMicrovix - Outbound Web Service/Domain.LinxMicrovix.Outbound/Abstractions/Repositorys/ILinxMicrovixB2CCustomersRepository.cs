using Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixB2CCustomersRepository
    {
        public Task<bool> BulkInsertIntoB2CConsultaClientes(List<B2CConsultaClientes> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaClientesContatos(List<B2CConsultaClientesContatos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaClientesContatosParentesco(List<B2CConsultaClientesContatosParentesco> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaClientesEnderecosEntrega(List<B2CConsultaClientesEnderecosEntrega> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaClientesEstadoCivil(List<B2CConsultaClientesEstadoCivil> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaClientesSaldo(List<B2CConsultaClientesSaldo> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaClientesSaldoLinx(List<B2CConsultaClientesSaldoLinx> data, Guid? parentExecutionGUID);
    }
}
