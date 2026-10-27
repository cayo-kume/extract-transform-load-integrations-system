namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosOpticosTipoAro: os tipos de aros (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosOpticosTipoAro
    {
        public string? id_tipo_aro { get; init; }
        public string? tipo_aro { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
