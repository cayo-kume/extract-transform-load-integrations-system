
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxProdutos
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? portal { get; private set; }
        public Int64? cod_produto { get; private set; }
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
        public Int32? cod_fornecedor { get; private set; }
        public string? desativado { get; private set; }
        public string? desc_espessura { get; private set; }
        public Int32? id_espessura { get; private set; }
        public string? desc_classificacao { get; private set; }
        public Int32? id_classificacao { get; private set; }
        public Int32? origem_mercadoria { get; private set; }
        public decimal? peso_liquido { get; private set; }
        public decimal? peso_bruto { get; private set; }
        public Int32? id_cor { get; private set; }
        public string? id_tamanho { get; private set; }
        public Int32? id_setor { get; private set; }
        public Int32? id_linha { get; private set; }
        public Int32? id_marca { get; private set; }
        public Int32? id_colecao { get; private set; }
        public DateTime? dt_inclusao { get; private set; }
        public Int64? timestamp { get; private set; }
        public decimal? fator_conversao { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public string? codigo_integracao_reshop { get; private set; }
        public Int32? id_produtos_opticos_tipo { get; private set; }
        public Int32? id_sped_tipo_item { get; private set; }
        public string? componente { get; private set; }
        public decimal? altura_para_frete { get; private set; }
        public decimal? largura_para_frete { get; private set; }
        public decimal? comprimento_para_frete { get; private set; }
        public string? loja_virtual { get; private set; }
        public Int32? cod_comprador { get; private set; }
        public bool? obrigatorio_identificacao_cliente { get; private set; }
        public string? descricao_basica { get; private set; }
        public string? curva { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxProdutos() { }

        public LinxProdutos(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutos record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.cod_fornecedor = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_fornecedor);
            this.id_espessura = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_espessura);
            this.id_classificacao = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_classificacao);
            this.origem_mercadoria = ConverterValidatorsExtensions.ConvertToInt32Validation(record.origem_mercadoria);
            this.id_cor = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_cor);
            this.id_setor = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_setor);
            this.id_linha = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_linha);
            this.id_marca = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_marca);
            this.id_colecao = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_colecao);
            this.id_produtos_opticos_tipo = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_produtos_opticos_tipo);
            this.id_sped_tipo_item = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_sped_tipo_item);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.cod_produto = ConverterValidatorsExtensions.ConvertToInt64Validation(record.cod_produto);
            this.dt_update = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_update);
            this.dt_inclusao = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_inclusao);
            this.peso_liquido = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.peso_liquido);
            this.peso_bruto = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.peso_bruto);
            this.fator_conversao = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.fator_conversao);
            this.altura_para_frete = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.altura_para_frete);
            this.largura_para_frete = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.largura_para_frete);
            this.comprimento_para_frete = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.comprimento_para_frete);
            this.obrigatorio_identificacao_cliente = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.obrigatorio_identificacao_cliente);
            this.curva = record.curva;
            this.descricao_basica = record.descricao_basica;
            this.loja_virtual = record.loja_virtual;
            this.componente = record.componente;
            this.codigo_integracao_reshop = record.codigo_integracao_reshop;
            this.codigo_integracao_ws = record.codigo_integracao_ws;
            this.id_tamanho = record.id_tamanho;
            this.desc_classificacao = record.desc_classificacao;
            this.desc_espessura = record.desc_espessura;
            this.desativado = record.desativado;
            this.desc_colecao = record.desc_colecao;
            this.desc_marca = record.desc_marca;
            this.desc_linha = record.desc_linha;
            this.desc_setor = record.desc_setor;
            this.desc_tamanho = record.desc_tamanho;
            this.desc_cor = record.desc_cor;
            this.unidade = record.unidade;
            this.cod_auxiliar = record.cod_auxiliar;
            this.referencia = record.referencia;
            this.cest = record.cest;
            this.ncm = record.ncm;
            this.nome = record.nome;
            this.cod_barra = record.cod_barra;
            this.recordKey = $"[{record.cod_produto}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
