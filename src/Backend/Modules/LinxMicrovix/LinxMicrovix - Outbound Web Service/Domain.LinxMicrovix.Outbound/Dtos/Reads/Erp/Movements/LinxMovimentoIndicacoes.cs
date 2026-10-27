namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMovimentoIndicacoes: o cliente indicador da venda. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMovimentoIndicacoes
    {
        public string? identificador { get; init; }
        public string? cod_cliente { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
