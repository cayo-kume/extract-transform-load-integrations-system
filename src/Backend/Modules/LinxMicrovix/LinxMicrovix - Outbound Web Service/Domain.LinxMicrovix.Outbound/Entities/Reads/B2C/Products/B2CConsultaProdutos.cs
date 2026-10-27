using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Products
{
    /// <summary>
    /// B2CConsultaProdutos, tabela linxmicrovix.B2CConsultaProdutos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + codigoproduto
    /// </summary>
    public class B2CConsultaProdutos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public long codigoproduto { get; private set; }
        public string? referencia { get; private set; }
        public string? codauxiliar1 { get; private set; }
        public string? descricao_basica { get; private set; }
        public string? nome_produto { get; private set; }
        public decimal? peso_liquido { get; private set; }
        public int? codigo_setor { get; private set; }
        public int? codigo_linha { get; private set; }
        public int? codigo_marca { get; private set; }
        public int? codigo_colecao { get; private set; }
        public int? codigo_espessura { get; private set; }
        public int? codigo_grade1 { get; private set; }
        public int? codigo_grade2 { get; private set; }
        public string? unidade { get; private set; }
        public bool? ativo { get; private set; }
        public int? codigo_classificacao { get; private set; }
        public DateTime? dt_cadastro { get; private set; }
        public string? observacao { get; private set; }
        public int? cod_fornecedor { get; private set; }
        public DateTime? dt_update { get; private set; }
        public long timestamp { get; private set; }
        public decimal? altura_para_frete { get; private set; }
        public decimal? largura_para_frete { get; private set; }
        public decimal? comprimento_para_frete { get; private set; }
        public decimal? peso_bruto { get; private set; }
        public int portal { get; private set; }
        public string? descricao_completa_commerce { get; private set; }
        public int? canais_ecommerce_publicados { get; private set; }
        public DateTime? inicio_publicacao_produto { get; private set; }
        public DateTime? fim_publicacao_produto { get; private set; }
        public string? codigo_integracao_oms { get; private set; }
        public string? descricao_garantia { get; private set; }
        public int? meses_garantia_fabrica { get; private set; }
        public string? ncm { get; private set; }
        public string? cest { get; private set; }
        public string? codigo_adicional_reshop { get; private set; }

        public B2CConsultaProdutos() { }

        public B2CConsultaProdutos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Products.B2CConsultaProdutos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            codigoproduto = MicrovixConverters.ToInt64(dto.codigoproduto);
            referencia = MicrovixConverters.ToNullableString(dto.referencia, 20);
            codauxiliar1 = MicrovixConverters.ToNullableString(dto.codauxiliar1, 40);
            descricao_basica = MicrovixConverters.ToNullableString(dto.descricao_basica, 100);
            nome_produto = MicrovixConverters.ToNullableString(dto.nome_produto, 250);
            peso_liquido = MicrovixConverters.ToNullableDecimal(dto.peso_liquido);
            codigo_setor = MicrovixConverters.ToNullableInt32(dto.codigo_setor);
            codigo_linha = MicrovixConverters.ToNullableInt32(dto.codigo_linha);
            codigo_marca = MicrovixConverters.ToNullableInt32(dto.codigo_marca);
            codigo_colecao = MicrovixConverters.ToNullableInt32(dto.codigo_colecao);
            codigo_espessura = MicrovixConverters.ToNullableInt32(dto.codigo_espessura);
            codigo_grade1 = MicrovixConverters.ToNullableInt32(dto.codigo_grade1);
            codigo_grade2 = MicrovixConverters.ToNullableInt32(dto.codigo_grade2);
            unidade = MicrovixConverters.ToNullableString(dto.unidade, 50);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            codigo_classificacao = MicrovixConverters.ToNullableInt32(dto.codigo_classificacao);
            dt_cadastro = MicrovixConverters.ToNullableDateTime(dto.dt_cadastro);
            observacao = String.IsNullOrEmpty(dto.observacao) ? null : dto.observacao;
            cod_fornecedor = MicrovixConverters.ToNullableInt32(dto.cod_fornecedor);
            dt_update = MicrovixConverters.ToNullableDateTime(dto.dt_update);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            altura_para_frete = MicrovixConverters.ToNullableDecimal(dto.altura_para_frete);
            largura_para_frete = MicrovixConverters.ToNullableDecimal(dto.largura_para_frete);
            comprimento_para_frete = MicrovixConverters.ToNullableDecimal(dto.comprimento_para_frete);
            peso_bruto = MicrovixConverters.ToNullableDecimal(dto.peso_bruto);
            portal = MicrovixConverters.ToInt32(dto.portal);
            descricao_completa_commerce = MicrovixConverters.ToNullableString(dto.descricao_completa_commerce, 8000);
            canais_ecommerce_publicados = MicrovixConverters.ToNullableInt32(dto.canais_ecommerce_publicados);
            inicio_publicacao_produto = MicrovixConverters.ToNullableDateTime(dto.inicio_publicacao_produto);
            fim_publicacao_produto = MicrovixConverters.ToNullableDateTime(dto.fim_publicacao_produto);
            codigo_integracao_oms = MicrovixConverters.ToNullableString(dto.codigo_integracao_oms, 50);
            descricao_garantia = String.IsNullOrEmpty(dto.descricao_garantia) ? null : dto.descricao_garantia;
            meses_garantia_fabrica = MicrovixConverters.ToNullableInt32(dto.meses_garantia_fabrica);
            ncm = MicrovixConverters.ToNullableString(dto.ncm, 20);
            cest = MicrovixConverters.ToNullableString(dto.cest, 10);
            codigo_adicional_reshop = MicrovixConverters.ToNullableString(dto.codigo_adicional_reshop, 100);
        }
    }
}
