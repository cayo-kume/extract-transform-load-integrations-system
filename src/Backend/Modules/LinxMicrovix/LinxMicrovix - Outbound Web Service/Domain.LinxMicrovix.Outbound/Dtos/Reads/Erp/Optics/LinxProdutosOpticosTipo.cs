namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosOpticosTipo: os tipos de produtos opticos. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosOpticosTipo
    {
        public string? id_produtos_opticos_tipo { get; init; }
        public string? tipo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
