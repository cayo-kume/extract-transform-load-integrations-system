namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaProdutos (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaProdutos
    {
        public string? codigoproduto { get; init; }
        public string? referencia { get; init; }
        public string? codauxiliar1 { get; init; }
        public string? descricao_basica { get; init; }
        public string? nome_produto { get; init; }
        public string? peso_liquido { get; init; }
        public string? codigo_setor { get; init; }
        public string? codigo_linha { get; init; }
        public string? codigo_marca { get; init; }
        public string? codigo_colecao { get; init; }
        public string? codigo_espessura { get; init; }
        public string? codigo_grade1 { get; init; }
        public string? codigo_grade2 { get; init; }
        public string? unidade { get; init; }
        public string? ativo { get; init; }
        public string? codigo_classificacao { get; init; }
        public string? dt_cadastro { get; init; }
        public string? observacao { get; init; }
        public string? cod_fornecedor { get; init; }
        public string? dt_update { get; init; }
        public string? timestamp { get; init; }
        public string? altura_para_frete { get; init; }
        public string? largura_para_frete { get; init; }
        public string? comprimento_para_frete { get; init; }
        public string? peso_bruto { get; init; }
        public string? portal { get; init; }
        public string? descricao_completa_commerce { get; init; }
        public string? canais_ecommerce_publicados { get; init; }
        public string? inicio_publicacao_produto { get; init; }
        public string? fim_publicacao_produto { get; init; }
        public string? codigo_integracao_oms { get; init; }
        public string? descricao_garantia { get; init; }
        public string? meses_garantia_fabrica { get; init; }
        public string? ncm { get; init; }
        public string? cest { get; init; }
        public string? codigo_adicional_reshop { get; init; }
    }
}
