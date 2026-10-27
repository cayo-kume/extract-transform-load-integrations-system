namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxLotesProdutos: os dados dos lotes dos produtos. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxLotesProdutos
    {
        public string? id_lote { get; init; }
        public string? codigo_produto { get; init; }
        public string? lote { get; init; }
        public string? identificador { get; init; }
        public string? transacao { get; init; }
        public string? data_fabricacao { get; init; }
        public string? data_vencimento { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
