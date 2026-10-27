namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosInventario: informações do saldo do produto na data pesquisada (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosInventario
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_produto { get; init; }
        public string? cod_barra { get; init; }
        public string? quantidade { get; init; }
        public string? empresa { get; init; }
        public string? timestamp { get; init; }
    }
}
