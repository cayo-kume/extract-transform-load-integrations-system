namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosTabelasPrecos: a relação dos produtos por tabela de preço (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosTabelasPrecos
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? id_tabela { get; init; }
        public string? cod_produto { get; init; }
        public string? precovenda { get; init; }
        public string? timestamp { get; init; }
    }
}
