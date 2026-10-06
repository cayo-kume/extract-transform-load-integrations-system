

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxProdutos
    {
        public string? portal { get; private set; }
        public string? cod_produto { get; private set; }
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
        public string? dt_update { get; private set; }
        public string? cod_fornecedor { get; private set; }
        public string? desativado { get; private set; }
        public string? desc_espessura { get; private set; }
        public string? id_espessura { get; private set; }
        public string? desc_classificacao { get; private set; }
        public string? id_classificacao { get; private set; }
        public string? origem_mercadoria { get; private set; }
        public string? peso_liquido { get; private set; }
        public string? peso_bruto { get; private set; }
        public string? id_cor { get; private set; }
        public string? id_tamanho { get; private set; }
        public string? id_setor { get; private set; }
        public string? id_linha { get; private set; }
        public string? id_marca { get; private set; }
        public string? id_colecao { get; private set; }
        public string? dt_inclusao { get; private set; }
        public string? timestamp { get; private set; }
        public string? fator_conversao { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public string? codigo_integracao_reshop { get; private set; }
        public string? id_produtos_opticos_tipo { get; private set; }
        public string? id_sped_tipo_item { get; private set; }
        public string? componente { get; private set; }
        public string? altura_para_frete { get; private set; }
        public string? largura_para_frete { get; private set; }
        public string? comprimento_para_frete { get; private set; }
        public string? loja_virtual { get; private set; }
        public string? cod_comprador { get; private set; }
        public string? obrigatorio_identificacao_cliente { get; private set; }
        public string? descricao_basica { get; private set; }
        public string? curva { get; private set; }

        public LinxProdutos() { }

        public LinxProdutos(
            string? portal,
            string? cod_produto,
            string? cod_barra,
            string? nome,
            string? ncm,
            string? cest,
            string? referencia,
            string? cod_auxiliar,
            string? unidade,
            string? desc_cor,
            string? desc_tamanho,
            string? desc_setor,
            string? desc_linha,
            string? desc_marca,
            string? desc_colecao,
            string? dt_update,
            string? cod_fornecedor,
            string? desativado,
            string? desc_espessura,
            string? id_espessura,
            string? desc_classificacao,
            string? id_classificacao,
            string? origem_mercadoria,
            string? peso_liquido,
            string? peso_bruto,
            string? id_cor,
            string? id_tamanho,
            string? id_setor,
            string? id_linha,
            string? id_marca,
            string? id_colecao,
            string? dt_inclusao,
            string? timestamp,
            string? fator_conversao,
            string? codigo_integracao_ws,
            string? codigo_integracao_reshop,
            string? id_produtos_opticos_tipo,
            string? id_sped_tipo_item,
            string? componente,
            string? altura_para_frete,
            string? largura_para_frete,
            string? comprimento_para_frete,
            string? loja_virtual,
            string? cod_comprador,
            string? obrigatorio_identificacao_cliente,
            string? descricao_basica,
            string? curva
        )
        {
            this.portal = portal;
            this.cod_produto = cod_produto;
            this.cod_barra = cod_barra;
            this.nome = nome;
            this.ncm = ncm;
            this.cest = cest;
            this.referencia = referencia;
            this.cod_auxiliar = cod_auxiliar;
            this.unidade = unidade;
            this.desc_cor = desc_cor;
            this.desc_tamanho = desc_tamanho;
            this.desc_setor = desc_setor;
            this.desc_linha = desc_linha;
            this.desc_marca = desc_marca;
            this.desc_colecao = desc_colecao;
            this.dt_update = dt_update;
            this.cod_fornecedor = cod_fornecedor;
            this.desativado = desativado;
            this.desc_espessura = desc_espessura;
            this.id_espessura = id_espessura;
            this.desc_classificacao = desc_classificacao;
            this.id_classificacao = id_classificacao;
            this.origem_mercadoria = origem_mercadoria;
            this.peso_liquido = peso_liquido;
            this.peso_bruto = peso_bruto;
            this.id_cor = id_cor;
            this.id_tamanho = id_tamanho;
            this.id_setor = id_setor;
            this.id_linha = id_linha;
            this.id_marca = id_marca;
            this.id_colecao = id_colecao;
            this.dt_inclusao = dt_inclusao;
            this.timestamp = timestamp;
            this.fator_conversao = fator_conversao;
            this.codigo_integracao_ws = codigo_integracao_ws;
            this.codigo_integracao_reshop = codigo_integracao_reshop;
            this.id_produtos_opticos_tipo = id_produtos_opticos_tipo;
            this.id_sped_tipo_item = id_sped_tipo_item;
            this.componente = componente;
            this.altura_para_frete = altura_para_frete;
            this.largura_para_frete = largura_para_frete;
            this.comprimento_para_frete = comprimento_para_frete;
            this.loja_virtual = loja_virtual;
            this.cod_comprador = cod_comprador;
            this.obrigatorio_identificacao_cliente = obrigatorio_identificacao_cliente;
            this.descricao_basica = descricao_basica;
            this.curva = curva;
            this.portal = portal;
            this.cod_produto = cod_produto;
            this.cod_barra = cod_barra;
            this.nome = nome;
            this.ncm = ncm;
            this.cest = cest;
            this.referencia = referencia;
            this.cod_auxiliar = cod_auxiliar;
            this.unidade = unidade;
            this.desc_cor = desc_cor;
            this.desc_tamanho = desc_tamanho;
            this.desc_setor = desc_setor;
            this.desc_linha = desc_linha;
            this.desc_marca = desc_marca;
            this.desc_colecao = desc_colecao;
            this.dt_update = dt_update;
            this.cod_fornecedor = cod_fornecedor;
            this.desativado = desativado;
            this.desc_espessura = desc_espessura;
            this.id_espessura = id_espessura;
            this.desc_classificacao = desc_classificacao;
            this.id_classificacao = id_classificacao;
            this.origem_mercadoria = origem_mercadoria;
            this.peso_liquido = peso_liquido;
            this.peso_bruto = peso_bruto;
            this.id_cor = id_cor;
            this.id_tamanho = id_tamanho;
            this.id_setor = id_setor;
            this.id_linha = id_linha;
            this.id_marca = id_marca;
            this.id_colecao = id_colecao;
            this.dt_inclusao = dt_inclusao;
            this.timestamp = timestamp;
            this.fator_conversao = fator_conversao;
            this.codigo_integracao_ws = codigo_integracao_ws;
            this.codigo_integracao_reshop = codigo_integracao_reshop;
            this.id_produtos_opticos_tipo = id_produtos_opticos_tipo;
            this.id_sped_tipo_item = id_sped_tipo_item;
            this.componente = componente;
            this.altura_para_frete = altura_para_frete;
            this.largura_para_frete = largura_para_frete;
            this.comprimento_para_frete = comprimento_para_frete;
            this.loja_virtual = loja_virtual;
            this.cod_comprador = cod_comprador;
            this.obrigatorio_identificacao_cliente = obrigatorio_identificacao_cliente;
            this.descricao_basica = descricao_basica;
            this.curva = curva;
        }
    }
}