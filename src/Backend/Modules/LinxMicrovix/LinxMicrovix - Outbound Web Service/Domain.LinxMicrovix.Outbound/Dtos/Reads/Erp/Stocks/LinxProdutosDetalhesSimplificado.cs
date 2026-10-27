namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosDetalhesSimplificado: os dados adicionais de produtos de forma simplificada (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosDetalhesSimplificado
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_produto { get; init; }
        public string? quantidade { get; init; }
        public string? id_config_tributaria { get; init; }
        public string? timestamp { get; init; }
    }
}
