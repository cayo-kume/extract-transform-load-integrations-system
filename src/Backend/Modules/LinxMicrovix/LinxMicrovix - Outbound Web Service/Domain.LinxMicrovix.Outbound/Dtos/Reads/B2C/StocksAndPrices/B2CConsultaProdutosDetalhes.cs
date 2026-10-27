namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosDetalhes (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosDetalhes
    {
        public string? id_prod_det { get; init; }
        public string? codigoproduto { get; init; }
        public string? empresa { get; init; }
        public string? saldo { get; init; }
        public string? controla_lote { get; init; }
        public string? nomeproduto_alternativo { get; init; }
        public string? timestamp { get; init; }
        public string? referencia { get; init; }
        public string? localizacao { get; init; }
        public string? portal { get; init; }
        public string? tempo_preparacao_estoque { get; init; }
    }
}
