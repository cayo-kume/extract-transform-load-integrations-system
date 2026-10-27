namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoObservacaoCF: as observações do cupom fiscal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoObservacaoCF
    {
        public string? identificador { get; init; }
        public string? documento_cliente { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
