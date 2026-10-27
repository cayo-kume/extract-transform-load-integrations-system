namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosDetalhesDepositos (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosDetalhesDepositos
    {
        public string? codigoproduto { get; init; }
        public string? empresa { get; init; }
        public string? id_deposito { get; init; }
        public string? saldo { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
        public string? tempo_preparacao_estoque { get; init; }
    }
}
