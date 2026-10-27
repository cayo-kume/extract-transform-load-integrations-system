using Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders;

namespace Domain.LinxMicrovix.Outbound.Abstractions.Repositorys
{
    public interface ILinxMicrovixB2COrdersRepository
    {
        public Task<bool> BulkInsertIntoB2CConsultaPedidos(List<B2CConsultaPedidos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaPedidosItens(List<B2CConsultaPedidosItens> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaPedidosPlanos(List<B2CConsultaPedidosPlanos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaPedidosStatus(List<B2CConsultaPedidosStatus> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaPedidosIdentificador(List<B2CConsultaPedidosIdentificador> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaPedidosTipos(List<B2CConsultaPedidosTipos> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaStatus(List<B2CConsultaStatus> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaCodigoRastreio(List<B2CConsultaCodigoRastreio> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaNFe(List<B2CConsultaNFe> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoB2CConsultaNFeSituacao(List<B2CConsultaNFeSituacao> data, Guid? parentExecutionGUID);
    }
}
