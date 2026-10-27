namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxEspessuras: as espessuras cadastradas. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxEspessuras
    {
        public string? id_espessura { get; init; }
        public string? desc_espessura { get; init; }
        public string? timestamp { get; init; }
        public string? codigo_integracao_ws { get; init; }
        public string? ativo { get; init; }
        public string? portal { get; init; }
    }
}
