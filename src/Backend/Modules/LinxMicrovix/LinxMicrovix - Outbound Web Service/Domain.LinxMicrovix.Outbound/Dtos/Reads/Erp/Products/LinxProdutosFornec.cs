namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutosFornec: os registros de produtos x fornecedores (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutosFornec
    {
        public string? id_prod_forn { get; init; }
        public string? codigoproduto { get; init; }
        public string? cod_fornecedor { get; init; }
        public string? custo { get; init; }
        public string? moeda { get; init; }
        public string? unidade_compra { get; init; }
        public string? fator_conversao_compra { get; init; }
        public string? codauxiliar { get; init; }
        public string? qtde_embalagem { get; init; }
        public string? prazo_entrega_padrao { get; init; }
        public string? empresa { get; init; }
        public string? desconto1 { get; init; }
        public string? desconto2 { get; init; }
        public string? desconto3 { get; init; }
        public string? desconto { get; init; }
        public string? ipi1 { get; init; }
        public string? diferencial_icms { get; init; }
        public string? despesas1 { get; init; }
        public string? acrescimo { get; init; }
        public string? valor_custo_substituicao { get; init; }
        public string? frete1 { get; init; }
        public string? fornecedor_principal { get; init; }
        public string? excluido { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
