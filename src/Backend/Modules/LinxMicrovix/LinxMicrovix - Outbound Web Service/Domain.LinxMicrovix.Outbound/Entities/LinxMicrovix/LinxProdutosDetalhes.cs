
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxProdutosDetalhes
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public Int64? cod_produto { get; private set; }
        public string? cod_barra { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? preco_custo { get; private set; }
        public decimal? preco_venda { get; private set; }
        public decimal? custo_medio { get; private set; }
        public Int32? id_config_tributaria { get; private set; }
        public string? desc_config_tributaria { get; private set; }
        public decimal? despesas1 { get; private set; }
        public decimal? qtde_minima { get; private set; }
        public decimal? qtde_maxima { get; private set; }
        public decimal? ipi { get; private set; }
        public Int64? timestamp { get; private set; }
        public Int32? empresa { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxProdutosDetalhes() { }

        public LinxProdutosDetalhes(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosDetalhes record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.id_config_tributaria = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_config_tributaria);
            this.empresa = ConverterValidatorsExtensions.ConvertToInt32Validation(record.empresa);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.cod_produto = ConverterValidatorsExtensions.ConvertToInt64Validation(record.cod_produto);
            this.quantidade = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.quantidade);
            this.preco_custo = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.preco_custo);
            this.preco_venda = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.preco_venda);
            this.custo_medio = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.custo_medio);
            this.despesas1 = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.despesas1);
            this.qtde_minima = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.qtde_minima);
            this.qtde_maxima = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.qtde_maxima);
            this.ipi = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.ipi);
            this.cnpj_emp = record.cnpj_emp;
            this.cod_barra = record.cod_barra;
            this.desc_config_tributaria = record.desc_config_tributaria;
            this.recordKey = $"[{record.cnpj_emp}]|[{record.cod_produto}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
