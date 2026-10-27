namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosOpticosFormatoAro: os formatos de aros (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosOpticosFormatoAro
    {
        public string? id_formato_aro { get; init; }
        public string? codigo { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
