namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosDetalhes: informações dos produtos de acordo com a loja pesquisada (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosDetalhes
    {
        public string? portal { get; init; }
        public string? cnpj_emp { get; init; }
        public string? cod_produto { get; init; }
        public string? cod_barra { get; init; }
        public string? quantidade { get; init; }
        public string? preco_custo { get; init; }
        public string? preco_venda { get; init; }
        public string? custo_medio { get; init; }
        public string? id_config_tributaria { get; init; }
        public string? desc_config_tributaria { get; init; }
        public string? despesas1 { get; init; }
        public string? qtde_minima { get; init; }
        public string? qtde_maxima { get; init; }
        public string? ipi { get; init; }
        public string? timestamp { get; init; }
        public string? custototal { get; init; }
        public string? empresa { get; init; }
        public string? id_produtos_status_compra { get; init; }
        public string? localizacao { get; init; }
    }
}
