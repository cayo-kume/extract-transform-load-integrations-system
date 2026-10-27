namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoVendaConjugada: movimento da venda conjugada (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoVendaConjugada
    {
        public string? identificador_produto { get; init; }
        public string? identificador_servico { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
