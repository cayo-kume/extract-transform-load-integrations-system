namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosSerial: o saldo atual dos seriais cadastrados no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosSerial
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? codigoproduto { get; init; }
        public string? serial { get; init; }
        public string? id_deposito { get; init; }
        public string? saldo { get; init; }
        public string? timestamp { get; init; }
    }
}
