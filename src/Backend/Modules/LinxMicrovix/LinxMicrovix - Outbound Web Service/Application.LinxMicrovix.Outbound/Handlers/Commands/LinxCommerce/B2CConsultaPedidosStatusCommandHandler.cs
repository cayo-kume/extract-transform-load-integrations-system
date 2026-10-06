using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxCommerce
{
    public class B2CConsultaPedidosStatusCommandHandler : IB2CConsultaPedidosStatusCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"{r}"));
            return $"SELECT CONCAT('[', ID, ']', '|', '[', ID_STATUS, ']', '|', '[', ID_PEDIDO, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_commerce].[B2CConsultaPedidosStatus] WHERE ID IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                          ([lastupdateon], [id], [id_status], [id_pedido], [data_hora], [anotacao], [timestamp], [portal]) 
                          Values 
                          (@lastupdateon, @id, @id_status, @id_pedido, @data_hora, @anotacao, @timestamp, @portal)";
        }

        public string CreateIntegrityLockQuery()
        {
            return @$"select distinct top 500 
                      'B2CConsultaPedidosStatus' as [table], 
                      'id_pedido' as recordKey, 
                      a.id_pedido as identifier 
                      from linx_microvix_commerce.b2cconsultapedidos a (nolock)
                      left join linx_microvix_commerce.b2cconsultapedidosstatus b (nolock) on a.id_pedido = b.id_pedido
                      left join linx_microvix.IntegrityLockTablesRegisters c (nolock) on a.id_pedido = c.identifier and c.[table] = 'B2CConsultaPedidosStatus'
                      where b.id_pedido is null and c.identifier is null and a.dt_pedido > '2025-01-01'";
        }
    }
}
