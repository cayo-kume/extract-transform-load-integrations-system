namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosDepositos: os depósitos cadastrados no ERP (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosDepositos
    {
        public string? portal { get; init; }
        public string? cod_deposito { get; init; }
        public string? nome_deposito { get; init; }
        public string? disponivel { get; init; }
        public string? disponivel_transferencia { get; init; }
        public string? timestamp { get; init; }
        public string? outlet { get; init; }
    }
}
