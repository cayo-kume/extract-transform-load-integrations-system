namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosTabelasPrecos (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosTabelasPrecos
    {
        public string? id_prod_tab_preco { get; init; }
        public string? id_tabela { get; init; }
        public string? codigoproduto { get; init; }
        public string? precovenda { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
