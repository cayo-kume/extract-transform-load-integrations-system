namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosCustos (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosCustos
    {
        public string? id_produtos_custos { get; init; }
        public string? codigoproduto { get; init; }
        public string? empresa { get; init; }
        public string? custoicms1 { get; init; }
        public string? ipi1 { get; init; }
        public string? markup { get; init; }
        public string? customedio { get; init; }
        public string? frete1 { get; init; }
        public string? precisao { get; init; }
        public string? precominimo { get; init; }
        public string? dt_update { get; init; }
        public string? custoliquido { get; init; }
        public string? precovenda { get; init; }
        public string? custototal { get; init; }
        public string? precocompra { get; init; }
        public string? timestamp { get; init; }
        public string? desconto_especial { get; init; }
        public string? portal { get; init; }
    }
}
