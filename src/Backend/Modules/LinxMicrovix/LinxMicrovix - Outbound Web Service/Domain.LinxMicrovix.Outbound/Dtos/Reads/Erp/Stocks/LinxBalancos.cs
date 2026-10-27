namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxBalancos: os balanços. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxBalancos
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? id_balancos { get; init; }
        public string? descricao { get; init; }
        public string? data_lancamento { get; init; }
        public string? status { get; init; }
        public string? id_deposito { get; init; }
        public string? timestamp { get; init; }
    }
}
