using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxCommerce
{
    public class B2CConsultaPedidosIdentificadorCommandHandler : IB2CConsultaPedidosIdentificadorCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<B2CConsultaPedidosIdentificador> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r.identificador}'"));
            return $"SELECT CONCAT('[', IDENTIFICADOR, ']', '|', '[', [TIMESTAMP], ']') FROM linx_microvix_commerce.B2CCONSULTAPEDIDOSIDENTIFICADOR WHERE IDENTIFICADOR IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO {tableName} 
                          ([lastupdateon], [portal], [empresa], [identificador], [id_venda], [order_id], [id_cliente], [valor_frete], [data_origem], [timestamp]) 
                          Values 
                          (@lastupdateon, @portal, @empresa, @identificador, @id_venda, @order_id, @id_cliente, @valor_frete, @data_origem, @timestamp)";
        }

        public string CreateIntegrityLockQuery()
        {
            return @$"select distinct top 500 
                      'B2CConsultaPedidosIdentificador' as [table], 
                      'order_id' as recordKey, 
                      a.id_pedido as identifier
                      from linx_microvix_commerce.b2cconsultapedidos a (nolock)
                      left join linx_microvix_commerce.b2cconsultapedidosidentificador b (nolock) on a.id_pedido = b.id_venda
                      left join linx_microvix.IntegrityLockTablesRegisters d (nolock) on a.id_pedido = d.identifier and [table] = 'B2CConsultaPedidosIdentificador'
                      where b.id_venda is null and d.identifier is null and a.dt_pedido > '2025-01-01'";
        }
    }
}
