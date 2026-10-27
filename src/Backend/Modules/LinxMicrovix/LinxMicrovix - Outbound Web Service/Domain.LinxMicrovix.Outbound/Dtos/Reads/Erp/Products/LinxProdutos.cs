namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxProdutos: o cadastro de produtos de acordo com a loja pesquisada ou dados da estrutura mercadológica (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxProdutos
    {
        public string? portal { get; init; }
        public string? cod_produto { get; init; }
        public string? cod_barra { get; init; }
        public string? nome { get; init; }
        public string? ncm { get; init; }
        public string? cest { get; init; }
        public string? referencia { get; init; }
        public string? cod_auxiliar { get; init; }
        public string? unidade { get; init; }
        public string? desc_cor { get; init; }
        public string? desc_tamanho { get; init; }
        public string? desc_setor { get; init; }
        public string? desc_linha { get; init; }
        public string? desc_marca { get; init; }
        public string? desc_colecao { get; init; }
        public string? dt_update { get; init; }
        public string? cod_fornecedor { get; init; }
        public string? desativado { get; init; }
        public string? desc_espessura { get; init; }
        public string? id_espessura { get; init; }
        public string? desc_classificacao { get; init; }
        public string? id_classificacao { get; init; }
        public string? origem_mercadoria { get; init; }
        public string? peso_liquido { get; init; }
        public string? peso_bruto { get; init; }
        public string? id_cor { get; init; }
        public string? id_tamanho { get; init; }
        public string? id_setor { get; init; }
        public string? id_linha { get; init; }
        public string? id_marca { get; init; }
        public string? id_colecao { get; init; }
        public string? dt_inclusao { get; init; }
        public string? timestamp { get; init; }
        public string? fator_conversao { get; init; }
        public string? codigo_integracao_ws { get; init; }
        public string? codigo_integracao_reshop { get; init; }
        public string? id_produtos_opticos_tipo { get; init; }
        public string? id_sped_tipo_item { get; init; }
        public string? componente { get; init; }
        public string? altura_para_frete { get; init; }
        public string? largura_para_frete { get; init; }
        public string? comprimento_para_frete { get; init; }
        public string? loja_virtual { get; init; }
        public string? codigoproduto_io { get; init; }
        public string? cod_comprador { get; init; }
        public string? codigo_integracao_oms { get; init; }
        public string? obrigatorio_identificacao_cliente { get; init; }
        public string? descricao_basica { get; init; }
        public string? curva { get; init; }
        public string? tributa_fcp { get; init; }
        public string? observacao { get; init; }
        public string? codigo_adicional_reshop { get; init; }
        public string? brinde { get; init; }
        public string? exige_controle { get; init; }
        public string? multiplo_venda { get; init; }
        public string? unidade_compra { get; init; }
    }
}
