namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxCentroCusto: as informações dos centros de custos cadastrados no portal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxCentroCusto
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? CNPJ { get; init; }
        public string? id_centrocusto { get; init; }
        public string? nome_centrocusto { get; init; }
        public string? ativo { get; init; }
        public string? timestamp { get; init; }
    }
}
