
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxProdutosInventario
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public Int64? cod_produto { get; private set; }
        public string? cod_barra { get; private set; }
        public decimal? quantidade { get; private set; }
        public Int32? cod_deposito { get; private set; }
        public Int32? empresa { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxProdutosInventario() { }

        public LinxProdutosInventario(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosInventario record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.cod_deposito = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_deposito);
            this.empresa = ConverterValidatorsExtensions.ConvertToInt32Validation(record.empresa);
            this.cod_produto = ConverterValidatorsExtensions.ConvertToInt64Validation(record.cod_produto);
            this.quantidade = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.quantidade);
            this.cnpj_emp = record.cnpj_emp;
            this.cod_barra = record.cod_barra;
            this.recordKey = $"[{record.cnpj_emp}]|[{record.cod_produto}]|[{record.cod_deposito}]";
            this.recordXml = recordXml;
        }
    }
}
