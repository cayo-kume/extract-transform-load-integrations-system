namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxLojasParametros: parâmetros do portal para a loja consultada (colunas da resposta real, 26/10/2026)
    /// </summary>
    public class LinxLojasParametros
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? percentual_minimo_antecipacao { get; init; }
        public string? timestamp { get; init; }
    }
}
