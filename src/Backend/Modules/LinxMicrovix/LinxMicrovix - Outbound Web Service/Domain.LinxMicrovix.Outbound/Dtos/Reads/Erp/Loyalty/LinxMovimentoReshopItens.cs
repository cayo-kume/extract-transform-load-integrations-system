namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Loyalty
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoReshopItens: movimento_campanha_reshop_item. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoReshopItens
    {
        public string? id_movimento_campanha_reshop_item { get; init; }
        public string? id_campanha { get; init; }
        public string? identificador { get; init; }
        public string? valor_unitario { get; init; }
        public string? valor_desconto_item { get; init; }
        public string? valor_total_desconto_item { get; init; }
        public string? quantidade { get; init; }
        public string? valor_original { get; init; }
        public string? timestamp { get; init; }
        public string? ordem { get; init; }
        public string? portal { get; init; }
    }
}
