namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxFaturas: as faturas a receber ou a pagar de uma loja (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxFaturas
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? codigo_fatura { get; init; }
        public string? data_emissao { get; init; }
        public string? cod_cliente { get; init; }
        public string? nome_cliente { get; init; }
        public string? data_vencimento { get; init; }
        public string? data_baixa { get; init; }
        public string? valor_fatura { get; init; }
        public string? valor_pago { get; init; }
        public string? valor_desconto { get; init; }
        public string? valor_juros { get; init; }
        public string? documento { get; init; }
        public string? serie { get; init; }
        public string? ecf { get; init; }
        public string? observacao { get; init; }
        public string? qtde_parcelas { get; init; }
        public string? ordem_parcela { get; init; }
        public string? receber_pagar { get; init; }
        public string? vendedor { get; init; }
        public string? excluido { get; init; }
        public string? cancelado { get; init; }
        public string? identificador { get; init; }
        public string? NSU { get; init; }
        public string? cod_autorizacao { get; init; }
        public string? documento_sem_tef { get; init; }
        public string? autorizacao_sem_tef { get; init; }
        public string? plano { get; init; }
        public string? conta_credito { get; init; }
        public string? conta_debito { get; init; }
        public string? conta_fluxo { get; init; }
        public string? cod_historico { get; init; }
        public string? forma_pgto { get; init; }
        public string? ordem_cartao { get; init; }
        public string? banco_codigo { get; init; }
        public string? banco_agencia { get; init; }
        public string? banco_conta { get; init; }
        public string? banco_autorizacao_garantidora { get; init; }
        public string? numero_bilhete_seguro { get; init; }
        public string? timestamp { get; init; }
        public string? empresa { get; init; }
        public string? id_categorias_financeiras { get; init; }
        public string? taxa_financeira { get; init; }
        public string? valor_abatimento { get; init; }
        public string? valor_multa { get; init; }
        public string? centrocusto { get; init; }
        public string? perc_taxa_adquirente { get; init; }
        public string? fatura_origem_importacao_erp { get; init; }
        public string? cod_portador { get; init; }
        public string? nsu_host { get; init; }
        public string? nsu_sitef { get; init; }
        public string? previsao { get; init; }
        public string? data_lanc { get; init; }
        public string? dt_insert { get; init; }
        public string? plano_pagamento_baixa { get; init; }
    }
}
