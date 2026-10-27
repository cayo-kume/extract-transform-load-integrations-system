using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Products
{
    /// <summary>
    /// LinxProdutos, tabela linxmicrovix.LinxProdutos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_produto
    /// </summary>
    public class LinxProdutos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public long cod_produto { get; private set; }
        public string? cod_barra { get; private set; }
        public string? nome { get; private set; }
        public string? ncm { get; private set; }
        public string? cest { get; private set; }
        public string? referencia { get; private set; }
        public string? cod_auxiliar { get; private set; }
        public string? unidade { get; private set; }
        public string? desc_cor { get; private set; }
        public string? desc_tamanho { get; private set; }
        public string? desc_setor { get; private set; }
        public string? desc_linha { get; private set; }
        public string? desc_marca { get; private set; }
        public string? desc_colecao { get; private set; }
        public DateTime? dt_update { get; private set; }
        public int? cod_fornecedor { get; private set; }
        public string? desativado { get; private set; }
        public string? desc_espessura { get; private set; }
        public int? id_espessura { get; private set; }
        public string? desc_classificacao { get; private set; }
        public int? id_classificacao { get; private set; }
        public int? origem_mercadoria { get; private set; }
        public decimal? peso_liquido { get; private set; }
        public decimal? peso_bruto { get; private set; }
        public int? id_cor { get; private set; }
        public string? id_tamanho { get; private set; }
        public int? id_setor { get; private set; }
        public int? id_linha { get; private set; }
        public int? id_marca { get; private set; }
        public int? id_colecao { get; private set; }
        public DateTime? dt_inclusao { get; private set; }
        public long timestamp { get; private set; }
        public decimal? fator_conversao { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public string? codigo_integracao_reshop { get; private set; }
        public int? id_produtos_opticos_tipo { get; private set; }
        public int? id_sped_tipo_item { get; private set; }
        public string? componente { get; private set; }
        public decimal? altura_para_frete { get; private set; }
        public decimal? largura_para_frete { get; private set; }
        public decimal? comprimento_para_frete { get; private set; }
        public string? loja_virtual { get; private set; }
        public string? codigoproduto_io { get; private set; }
        public int? cod_comprador { get; private set; }
        public string? codigo_integracao_oms { get; private set; }
        public bool? obrigatorio_identificacao_cliente { get; private set; }
        public string? descricao_basica { get; private set; }
        public string? curva { get; private set; }
        public bool? tributa_fcp { get; private set; }
        public string? observacao { get; private set; }
        public string? codigo_adicional_reshop { get; private set; }
        public bool? brinde { get; private set; }
        public string? exige_controle { get; private set; }
        public decimal? multiplo_venda { get; private set; }
        public string? unidade_compra { get; private set; }

        public LinxProdutos() { }

        public LinxProdutos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Products.LinxProdutos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            cod_produto = MicrovixConverters.ToInt64(dto.cod_produto);
            cod_barra = MicrovixConverters.ToNullableString(dto.cod_barra, 20);
            nome = MicrovixConverters.ToNullableString(dto.nome, 250);   //fora da documentação
            ncm = MicrovixConverters.ToNullableString(dto.ncm, 20);
            cest = MicrovixConverters.ToNullableString(dto.cest, 10);
            referencia = MicrovixConverters.ToNullableString(dto.referencia, 20);
            cod_auxiliar = MicrovixConverters.ToNullableString(dto.cod_auxiliar, 40);
            unidade = MicrovixConverters.ToNullableString(dto.unidade, 50);
            desc_cor = MicrovixConverters.ToNullableString(dto.desc_cor, 30);
            desc_tamanho = MicrovixConverters.ToNullableString(dto.desc_tamanho, 50);
            desc_setor = MicrovixConverters.ToNullableString(dto.desc_setor, 30);
            desc_linha = MicrovixConverters.ToNullableString(dto.desc_linha, 30);
            desc_marca = MicrovixConverters.ToNullableString(dto.desc_marca, 30);
            desc_colecao = MicrovixConverters.ToNullableString(dto.desc_colecao, 50);
            dt_update = MicrovixConverters.ToNullableDateTime(dto.dt_update);
            cod_fornecedor = MicrovixConverters.ToNullableInt32(dto.cod_fornecedor);
            desativado = MicrovixConverters.ToNullableString(dto.desativado, 1);
            desc_espessura = MicrovixConverters.ToNullableString(dto.desc_espessura, 50);
            id_espessura = MicrovixConverters.ToNullableInt32(dto.id_espessura);
            desc_classificacao = MicrovixConverters.ToNullableString(dto.desc_classificacao, 50);
            id_classificacao = MicrovixConverters.ToNullableInt32(dto.id_classificacao);
            origem_mercadoria = MicrovixConverters.ToNullableInt32(dto.origem_mercadoria);
            peso_liquido = MicrovixConverters.ToNullableDecimal(dto.peso_liquido);
            peso_bruto = MicrovixConverters.ToNullableDecimal(dto.peso_bruto);
            id_cor = MicrovixConverters.ToNullableInt32(dto.id_cor);
            id_tamanho = MicrovixConverters.ToNullableString(dto.id_tamanho, 5);
            id_setor = MicrovixConverters.ToNullableInt32(dto.id_setor);
            id_linha = MicrovixConverters.ToNullableInt32(dto.id_linha);
            id_marca = MicrovixConverters.ToNullableInt32(dto.id_marca);
            id_colecao = MicrovixConverters.ToNullableInt32(dto.id_colecao);
            dt_inclusao = MicrovixConverters.ToNullableDateTime(dto.dt_inclusao);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            fator_conversao = MicrovixConverters.ToNullableDecimal(dto.fator_conversao);
            codigo_integracao_ws = MicrovixConverters.ToNullableString(dto.codigo_integracao_ws, 50);
            codigo_integracao_reshop = MicrovixConverters.ToNullableString(dto.codigo_integracao_reshop, 50);
            id_produtos_opticos_tipo = MicrovixConverters.ToNullableInt32(dto.id_produtos_opticos_tipo);
            id_sped_tipo_item = MicrovixConverters.ToNullableInt32(dto.id_sped_tipo_item);
            componente = MicrovixConverters.ToNullableString(dto.componente, 1);
            altura_para_frete = MicrovixConverters.ToNullableDecimal(dto.altura_para_frete);
            largura_para_frete = MicrovixConverters.ToNullableDecimal(dto.largura_para_frete);
            comprimento_para_frete = MicrovixConverters.ToNullableDecimal(dto.comprimento_para_frete);
            loja_virtual = String.IsNullOrEmpty(dto.loja_virtual) ? null : dto.loja_virtual;
            codigoproduto_io = MicrovixConverters.ToNullableString(dto.codigoproduto_io, 50);
            cod_comprador = MicrovixConverters.ToNullableInt32(dto.cod_comprador);
            codigo_integracao_oms = MicrovixConverters.ToNullableString(dto.codigo_integracao_oms, 50);
            obrigatorio_identificacao_cliente = MicrovixConverters.ToNullableBoolean(dto.obrigatorio_identificacao_cliente);
            descricao_basica = MicrovixConverters.ToNullableString(dto.descricao_basica, 100);
            curva = MicrovixConverters.ToNullableString(dto.curva, 1);
            tributa_fcp = MicrovixConverters.ToNullableBoolean(dto.tributa_fcp);
            observacao = MicrovixConverters.ToNullableString(dto.observacao, 1000);
            codigo_adicional_reshop = MicrovixConverters.ToNullableString(dto.codigo_adicional_reshop, 100);
            brinde = MicrovixConverters.ToNullableBoolean(dto.brinde);
            exige_controle = MicrovixConverters.ToNullableString(dto.exige_controle, 1);
            multiplo_venda = MicrovixConverters.ToNullableDecimal(dto.multiplo_venda);
            unidade_compra = MicrovixConverters.ToNullableString(dto.unidade_compra, 50);
        }
    }
}
