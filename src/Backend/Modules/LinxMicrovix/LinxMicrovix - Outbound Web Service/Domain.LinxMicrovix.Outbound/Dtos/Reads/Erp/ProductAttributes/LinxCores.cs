namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCores: as cores cadastradas. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCores
    {
        public string? id_cor { get; init; }
        public string? desc_cor { get; init; }
        public string? timestamp { get; init; }
        public string? codigo_integracao_ws { get; init; }
        public string? ativo { get; init; }
        public string? portal { get; init; }
    }
}
