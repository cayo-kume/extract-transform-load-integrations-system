namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxSetores: os setores cadastrados (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxSetores
    {
        public string? id_setor { get; init; }
        public string? desc_setor { get; init; }
        public string? timestamp { get; init; }
        public string? codigo_integracao_ws { get; init; }
        public string? ativo { get; init; }
        public string? portal { get; init; }
    }
}
