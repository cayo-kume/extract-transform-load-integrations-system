namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxVendasPosProdutosTmp: os registros da tabela vendas_pos_produtos_tmp. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxVendasPosProdutosTmp
    {
        public string? portal { get; init; }
        public string? id_vendas_pos_produtos { get; init; }
        public string? numero_ficha { get; init; }
        public string? empresa { get; init; }
        public string? codigoproduto { get; init; }
        public string? preco_unitario { get; init; }
        public string? preco_tabela { get; init; }
        public string? quantidade { get; init; }
        public string? vendedor { get; init; }
        public string? desconto_item { get; init; }
        public string? id_tabela_preco { get; init; }
        public string? altura { get; init; }
        public string? comprimento { get; init; }
        public string? largura { get; init; }
        public string? cfop_item { get; init; }
        public string? excluido { get; init; }
        public string? aliq_icms { get; init; }
        public string? cst_fiscal { get; init; }
        public string? indice_ordenacao_espelho { get; init; }
        public string? id_vendas_pos { get; init; }
        public string? id_cst_icms_fiscal { get; init; }
        public string? id_cst_ipi_fiscal { get; init; }
        public string? id_cst_pis_fiscal { get; init; }
        public string? id_cst_cofins_fiscal { get; init; }
        public string? aliq_iss { get; init; }
        public string? id_cfop_fiscal { get; init; }
        public string? aliq_pis { get; init; }
        public string? aliq_cofins { get; init; }
        public string? desconto_item_valor { get; init; }
        public string? lote { get; init; }
        public string? deposito { get; init; }
        public string? previsao_entrega { get; init; }
        public string? transacao_movimento_reserva { get; init; }
        public string? data_exclusao { get; init; }
        public string? desconto_total_item { get; init; }
        public string? id_motivo_desconto { get; init; }
        public string? motivo_desconto_observacao { get; init; }
        public string? crm_cliente_indicador { get; init; }
        public string? cod_cliente_indicador { get; init; }
        public string? timestamp { get; init; }
    }
}
