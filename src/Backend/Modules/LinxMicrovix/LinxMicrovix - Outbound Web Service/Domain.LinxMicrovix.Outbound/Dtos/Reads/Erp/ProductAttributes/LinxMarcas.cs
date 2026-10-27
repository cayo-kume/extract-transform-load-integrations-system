namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMarcas: as marcas cadastradas. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMarcas
    {
        public string? id_marca { get; init; }
        public string? desc_marca { get; init; }
        public string? timestamp { get; init; }
        public string? codigo_integracao_ws { get; init; }
        public string? portal { get; init; }
    }
}
