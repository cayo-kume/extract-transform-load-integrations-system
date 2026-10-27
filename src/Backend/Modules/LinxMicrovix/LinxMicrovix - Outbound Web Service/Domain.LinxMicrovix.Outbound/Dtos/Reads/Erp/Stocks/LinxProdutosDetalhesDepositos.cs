namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosDetalhesDepositos: os saldos dos produtos por depósito. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosDetalhesDepositos
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_produto { get; init; }
        public string? empresa { get; init; }
        public string? cod_deposito { get; init; }
        public string? saldo { get; init; }
        public string? timestamp { get; init; }
    }
}
