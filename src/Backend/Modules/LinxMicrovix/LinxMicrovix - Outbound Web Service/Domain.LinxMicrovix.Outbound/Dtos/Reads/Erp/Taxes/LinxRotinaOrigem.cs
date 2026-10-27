namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxRotinaOrigem: retornas as rotinas origem cadastradas. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxRotinaOrigem
    {
        public string? portal { get; init; }
        public string? codigo_rotina { get; init; }
        public string? descricao_rotina { get; init; }
        public string? timestamp { get; init; }
    }
}
