namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosDepositos (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosDepositos
    {
        public string? id_deposito { get; init; }
        public string? nome_deposito { get; init; }
        public string? disponivel { get; init; }
        public string? disponivel_transferencia { get; init; }
        public string? disponivel_franquias { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
        public string? outlet { get; init; }
    }
}
