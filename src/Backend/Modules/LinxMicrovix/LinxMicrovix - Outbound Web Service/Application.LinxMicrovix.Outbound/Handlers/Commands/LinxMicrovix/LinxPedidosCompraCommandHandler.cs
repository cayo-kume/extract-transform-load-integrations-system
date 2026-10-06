using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxPedidosCompraCommandHandler : ILinxPedidosCompraCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', CNPJ_EMP, ']', '|', '[', COD_PEDIDO, ']', '|', '[', CODIGO_FORNECEDOR, ']', '|', '[', COD_PRODUTO, ']', '|',  '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxPedidosCompra] WHERE COD_PEDIDO IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}]
                            ([lastupdateon],[portal],[cnpj_emp],[cod_pedido],[data_pedido],[transacao],[usuario],[codigo_fornecedor],[cod_produto],[quantidade],[valor_unitario],
                             [cod_comprador],[valor_frete],[valor_total],[cod_plano_pagamento],[plano_pagamento],[obs],[aprovado],[cancelado],[encerrado],[data_aprovacao],
                             [numero_ped_fornec],[tipo_frete],[natureza_operacao],[previsao_entrega],[numero_projeto_officina],[status_pedido],[qtde_entregue],[descricao_frete],
                             [integrado_linx],[nf_gerada],[timestamp],[empresa],[nf_origem_ws])
                            Values
                            (@lastupdateon,@portal,@cnpj_emp,@cod_pedido,@data_pedido,@transacao,@usuario,@codigo_fornecedor,@cod_produto,@quantidade,@valor_unitario,@cod_comprador,
                             @valor_frete,@valor_total,@cod_plano_pagamento,@plano_pagamento,@obs,@aprovado,@cancelado,@encerrado,@data_aprovacao,@numero_ped_fornec,@tipo_frete,
                             @natureza_operacao,@previsao_entrega,@numero_projeto_officina,@status_pedido,@qtde_entregue,@descricao_frete,@integrado_linx,@nf_gerada,@timestamp,
                             @empresa,@nf_origem_ws)";
        }
    }
}
