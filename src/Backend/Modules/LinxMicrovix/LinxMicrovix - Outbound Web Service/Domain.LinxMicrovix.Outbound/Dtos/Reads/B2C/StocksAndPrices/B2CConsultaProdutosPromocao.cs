namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutosPromocao (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutosPromocao
    {
        public string? codigo_promocao { get; init; }
        public string? empresa { get; init; }
        public string? codigoproduto { get; init; }
        public string? preco { get; init; }
        public string? data_inicio { get; init; }
        public string? data_termino { get; init; }
        public string? data_cadastro { get; init; }
        public string? ativa { get; init; }
        public string? codigo_campanha { get; init; }
        public string? promocao_opcional { get; init; }
        public string? timestamp { get; init; }
        public string? referencia { get; init; }
        public string? portal { get; init; }
    }
}
