using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxPedidosVendaCommandHandler : ILinxPedidosVendaCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', CNPJ_EMP, ']', '|', '[', COD_PEDIDO, ']', '|', '[', COD_PRODUTO, ']', '|', '[', CODIGO_CLIENTE, ']', '|',  '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxPedidosVenda] WHERE COD_PEDIDO IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}]
                            ([lastupdateon],[portal],[cnpj_emp],[cod_pedido],[data_lancamento],[hora_lancamento],[transacao],[usuario],[codigo_cliente],[cod_produto],[quantidade],
                             [valor_unitario],[cod_vendedor],[valor_frete],[valor_total],[desconto_item],[cod_plano_pagamento],[plano_pagamento],[obs],[aprovado],[cancelado],[data_aprovacao],
                             [data_alteracao],[tipo_frete],[natureza_operacao],[tabela_preco],[nome_tabela_preco],[previsao_entrega],[realizado_por],[pontuacao_ser],[venda_externa],
                             [nf_gerada],[status],[numero_projeto_officina],[cod_natureza_operacao],[margem_contribuicao],[doc_origem],[posicao_item],[orcamento_origem],[transacao_origem],
                             [timestamp],[desconto],[transacao_ws],[empresa],[transportador],[deposito])
                            Values
                            (@lastupdateon,@portal,@cnpj_emp,@cod_pedido,@data_lancamento,@hora_lancamento,@transacao,@usuario,@codigo_cliente,@cod_produto,@quantidade,@valor_unitario,@cod_vendedor,
                             @valor_frete,@valor_total,@desconto_item,@cod_plano_pagamento,@plano_pagamento,@obs,@aprovado,@cancelado,@data_aprovacao,@data_alteracao,@tipo_frete,@natureza_operacao,
                             @tabela_preco,@nome_tabela_preco,@previsao_entrega,@realizado_por,@pontuacao_ser,@venda_externa,@nf_gerada,@status,@numero_projeto_officina,@cod_natureza_operacao,
                             @margem_contribuicao,@doc_origem,@posicao_item,@orcamento_origem,@transacao_origem,@timestamp,@desconto,@transacao_ws,@empresa,@transportador,@deposito)";
        }
    }
}
