using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxNaturezaOperacaoCommandHandler : ILinxNaturezaOperacaoCommandHandler
    {
        public string CreateGetRegistersExistsQuery()
        {
            return "SELECT CONCAT('[', TRIM(COD_NATUREZA_OPERACAO), ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxNaturezaOperacao]";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                            ([lastupdateon],[portal],[cod_natureza_operacao],[descricao],[soma_relatorios],[operacao],[inativa],[timestamp],[calcula_ipi],[calcula_iss],[calcula_irrf],
                             [tipo_preco],[atualiza_custo],[transferencia],[baixar_estoque],[consumo_proprio],[contabiliza_cmv],[despesa],[atualiza_custo_medio],[exige_nf_origem],
                             [integra_contabilidade],[id_obs],[venda_futura],[base_icms_considera_ipi],[permite_escolha_historico],[import_produtos],[deposito_reserva_venda],[exibe_nfe],
                             [faturamento_antecipado],[exibir_informacoes_imposto],[gera_garantia_nacional],[transferencia_deposito],[venda_diferencial_aliquota],[insere_obs_pis_cofins],
                             [diferencial_ativo_consumo],[recusa_de],[codigo_ws])
                            Values
                            (@lastupdateon,@portal,@cod_natureza_operacao,@descricao,@soma_relatorios,@operacao,@inativa,@timestamp,@calcula_ipi,@calcula_iss,@calcula_irrf,@tipo_preco,
                             @atualiza_custo,@transferencia,@baixar_estoque,@consumo_proprio,@contabiliza_cmv,@despesa,@atualiza_custo_medio,@exige_nf_origem,@integra_contabilidade,
                             @id_obs,@venda_futura,@base_icms_considera_ipi,@permite_escolha_historico,@import_produtos,@deposito_reserva_venda,@exibe_nfe,@faturamento_antecipado,
                             @exibir_informacoes_imposto,@gera_garantia_nacional,@transferencia_deposito,@venda_diferencial_aliquota,@insere_obs_pis_cofins,@diferencial_ativo_consumo,
                             @recusa_de,@codigo_ws)";
        }
    }
}
