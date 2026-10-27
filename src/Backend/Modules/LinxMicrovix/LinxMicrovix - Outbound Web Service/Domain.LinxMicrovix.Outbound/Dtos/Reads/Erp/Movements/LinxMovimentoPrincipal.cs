namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoPrincipal: dados da movimento principal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoPrincipal
    {
        public string? id_movimento_principal { get; init; }
        public string? identificador { get; init; }
        public string? codigoproduto_manutencao { get; init; }
        public string? timestamp { get; init; }
        public string? id_pergunta_venda { get; init; }
        public string? id_resposta_venda { get; init; }
        public string? total_fidelidade_cashback { get; init; }
        public string? plano_fidelidade_cashback { get; init; }
        public string? remessa_pedido_compra { get; init; }
        public string? id_motivo_desconto { get; init; }
        public string? valor_nota { get; init; }
        public string? portal { get; init; }
        public string? estornada { get; init; }
        public string? motivo_desconto_observacao { get; init; }
        public string? nota_aguardou_sefaz { get; init; }
    }
}
