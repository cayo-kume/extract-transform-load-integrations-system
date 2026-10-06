using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxMovimentoCommandHandler : ILinxMovimentoCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<LinxMovimento> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r.identificador}'"));
            return $"SELECT CONCAT('[', CNPJ_EMP, ']', '|', '[', DOCUMENTO, ']', '|', '[', DATA_DOCUMENTO, ']', '|', '[', CODIGO_CLIENTE, ']', '|', '[', COD_PRODUTO, ']', '|', '[', CANCELADO, ']', '|', '[', EXCLUIDO, ']', '|', '[', TRANSACAO_PEDIDO_VENDA, ']', '|', '[', ORDEM, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxMovimento] WHERE identificador IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].{tableName} 
                            ([lastupdateon],[portal],[cnpj_emp],[transacao],[usuario],[documento],[chave_nf],[ecf],[numero_serie_ecf],[modelo_nf],[data_documento],[data_lancamento],
                             [codigo_cliente],[serie],[desc_cfop],[id_cfop],[cod_vendedor],[quantidade],[preco_custo],[valor_liquido],[desconto],[cst_icms],[cst_pis],[cst_cofins],
                             [cst_ipi],[valor_icms],[aliquota_icms],[base_icms],[valor_pis],[aliquota_pis],[base_pis],[valor_cofins],[aliquota_cofins],[base_cofins],[valor_icms_st],
                             [aliquota_icms_st],[base_icms_st],[valor_ipi],[aliquota_ipi],[base_ipi],[valor_total],[forma_dinheiro],[total_dinheiro],[forma_cheque],[total_cheque],
                             [forma_cartao],[total_cartao],[forma_crediario],[total_crediario],[forma_convenio],[total_convenio],[frete],[operacao],[tipo_transacao],[cod_produto],
                             [cod_barra],[cancelado],[excluido],[soma_relatorio],[identificador],[deposito],[obs],[preco_unitario],[hora_lancamento],[natureza_operacao],[tabela_preco],
                             [nome_tabela_preco],[cod_sefaz_situacao],[desc_sefaz_situacao],[protocolo_aut_nfe],[dt_update],[forma_cheque_prazo],[total_cheque_prazo],[cod_natureza_operacao],
                             [preco_tabela_epoca],[desconto_total_item],[conferido],[transacao_pedido_venda],[codigo_modelo_nf],[acrescimo],[mob_checkout],[aliquota_iss],[base_iss],
                             [ordem],[codigo_rotina_origem],[timestamp],[troco],[transportador],[icms_aliquota_desonerado],[icms_valor_desonerado_item],[empresa],[desconto_item],
                             [aliq_iss],[iss_base_item],[despesas],[seguro_total_item],[acrescimo_total_item],[despesas_total_item],[forma_pix],[total_pix],[forma_deposito_bancario],
                             [total_deposito_bancario],[id_venda_produto_b2c],[item_promocional],[acrescimo_item],[icms_st_antecipado_aliquota],[icms_st_antecipado_margem],[icms_st_antecipado_percentual_reducao],
                             [icms_st_antecipado_valor_item],[icms_base_desonerado_item],[codigo_status_nfe])
                           Values
                            (@lastupdateon,@portal,@cnpj_emp,@transacao,@usuario,@documento,@chave_nf,@ecf,@numero_serie_ecf,@modelo_nf,@data_documento,@data_lancamento,@codigo_cliente,@serie,
                             @desc_cfop,@id_cfop,@cod_vendedor,@quantidade,@preco_custo,@valor_liquido,@desconto,@cst_icms,@cst_pis,@cst_cofins,@cst_ipi,@valor_icms,@aliquota_icms,@base_icms,
                             @valor_pis,@aliquota_pis,@base_pis,@valor_cofins,@aliquota_cofins,@base_cofins,@valor_icms_st,@aliquota_icms_st,@base_icms_st,@valor_ipi,@aliquota_ipi,@base_ipi,
                             @valor_total,@forma_dinheiro,@total_dinheiro,@forma_cheque,@total_cheque,@forma_cartao,@total_cartao,@forma_crediario,@total_crediario,@forma_convenio,@total_convenio,
                             @frete,@operacao,@tipo_transacao,@cod_produto,@cod_barra,@cancelado,@excluido,@soma_relatorio,@identificador,@deposito,@obs,@preco_unitario,@hora_lancamento,@natureza_operacao,
                             @tabela_preco,@nome_tabela_preco,@cod_sefaz_situacao,@desc_sefaz_situacao,@protocolo_aut_nfe,@dt_update,@forma_cheque_prazo,@total_cheque_prazo,@cod_natureza_operacao,
                             @preco_tabela_epoca,@desconto_total_item,@conferido,@transacao_pedido_venda,@codigo_modelo_nf,@acrescimo,@mob_checkout,@aliquota_iss,@base_iss,@ordem,@codigo_rotina_origem,
                             @timestamp,@troco,@transportador,@icms_aliquota_desonerado,@icms_valor_desonerado_item,@empresa,@desconto_item,@aliq_iss,@iss_base_item,@despesas,@seguro_total_item,@acrescimo_total_item,
                             @despesas_total_item,@forma_pix,@total_pix,@forma_deposito_bancario,@total_deposito_bancario,@id_venda_produto_b2c,@item_promocional,@acrescimo_item,
                             @icms_st_antecipado_aliquota,@icms_st_antecipado_margem,@icms_st_antecipado_percentual_reducao,@icms_st_antecipado_valor_item,@icms_base_desonerado_item,@codigo_status_nfe)";
        }
    }
}
