using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxCommerce
{
    public class B2CConsultaPedidosCommandHandler : IB2CConsultaPedidosCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', EMPRESA, ']', '|', '[', ID_PEDIDO, ']', '|', '[', COD_CLIENTE_B2C, ']', '|', '[', COD_CLIENTE_ERP, ']', '|', '[', ORDER_ID, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_commerce].[B2CCONSULTAPEDIDOS] WHERE ID_PEDIDO IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                          ([lastupdateon], [id_pedido], [dt_pedido], [cod_cliente_erp], [cod_cliente_b2c], [vl_frete], [forma_pgto], [plano_pagamento], [anotacao], [taxa_impressao], [finalizado], [valor_frete_gratis], [tipo_frete], 
                          [id_status], [cod_transportador], [tipo_cobranca_frete], [ativo], [empresa], [id_tabela_preco], [valor_credito], [cod_vendedor], [timestamp], [dt_insert], [dt_disponivel_faturamento], [portal], 
                          [mensagem_falha_faturamento], [id_tipo_b2c], [ecommerce_origem], [order_id]) 
                          Values 
                          (@lastupdateon, @id_pedido, @dt_pedido, @cod_cliente_erp, @cod_cliente_b2c, @vl_frete, @forma_pgto, @plano_pagamento, @anotacao, @taxa_impressao, @finalizado, @valor_frete_gratis, @tipo_frete, @id_status, 
                          @cod_transportador, @tipo_cobranca_frete, @ativo, @empresa, @id_tabela_preco, @valor_credito, @cod_vendedor, @timestamp, @dt_insert, @dt_disponivel_faturamento, @portal, @mensagem_falha_faturamento, @id_tipo_b2c, 
                          @ecommerce_origem, @order_id)";
        }

        public string CreateIntegrityLockQuery()
        {
            return @$"select distinct top 500 
                      'B2CConsultaPedidos' as [table], 
                      'order_id' as recordKey, 
                      a.ordernumber as identifier 
                      from linx_commerce.[order] a (nolock)
                      left join linx_microvix_commerce.b2cconsultapedidos b (nolock) on a.ordernumber = b.order_id or IIF(a.OrderStatusID = 6, concat(a.ordernumber, '-CANCELLED'), a.ordernumber) = b.order_id
                      left join linx_microvix.IntegrityLockTablesRegisters d (nolock) on a.ordernumber = d.identifier and [table] = 'B2CConsultaPedidos'
                      where b.order_id is null and d.identifier is null and a.createddate > '2025-01-01'";
        }
    }
}
