namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoSerial: os Código de Seriais realizadas nas movimentações (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoSerial
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? identificador { get; init; }
        public string? transacao { get; init; }
        public string? serial { get; init; }
        public string? timestamp { get; init; }
    }
}
