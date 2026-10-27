namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxEntradaNFeProdutos: dados de itens de NFes de entrada. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxEntradaNFeProdutos
    {
        public string? id_entrada_nfe_produtos { get; init; }
        public string? id_entrada_nfe { get; init; }
        public string? numero_item { get; init; }
        public string? codigo_produto { get; init; }
        public string? descricao_produto { get; init; }
        public string? gtin { get; init; }
        public string? qtde_tributavel { get; init; }
        public string? qtde_comercial { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
