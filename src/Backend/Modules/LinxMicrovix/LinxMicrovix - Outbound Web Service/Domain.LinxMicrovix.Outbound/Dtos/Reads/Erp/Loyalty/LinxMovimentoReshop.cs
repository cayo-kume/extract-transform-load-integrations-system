namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Loyalty
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoReshop: movimento_campanha_reshop. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoReshop
    {
        public string? id_movimento_campanha_reshop { get; init; }
        public string? id_campanha { get; init; }
        public string? identificador { get; init; }
        public string? nome { get; init; }
        public string? descricao { get; init; }
        public string? aplicar_desconto_venda { get; init; }
        public string? valor_desconto_subtotal { get; init; }
        public string? valor_desconto_completo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
