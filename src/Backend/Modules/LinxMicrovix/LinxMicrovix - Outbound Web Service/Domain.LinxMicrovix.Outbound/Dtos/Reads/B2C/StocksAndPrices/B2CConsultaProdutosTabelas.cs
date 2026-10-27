namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosTabelas (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosTabelas
    {
        public string? id_tabela { get; init; }
        public string? nome_tabela { get; init; }
        public string? ativa { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
