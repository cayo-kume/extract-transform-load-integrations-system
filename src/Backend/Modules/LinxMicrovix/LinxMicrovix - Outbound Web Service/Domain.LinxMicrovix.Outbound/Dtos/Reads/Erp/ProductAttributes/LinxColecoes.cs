namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxColecoes: as coleções cadastradas. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxColecoes
    {
        public string? id_colecao { get; init; }
        public string? desc_colecao { get; init; }
        public string? timestamp { get; init; }
        public string? codigo_integracao_ws { get; init; }
        public string? portal { get; init; }
    }
}
