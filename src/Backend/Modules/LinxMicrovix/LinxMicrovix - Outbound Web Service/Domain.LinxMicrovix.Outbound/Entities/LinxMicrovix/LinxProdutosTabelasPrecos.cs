
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxProdutosTabelasPrecos
    {
        public DateTime? lastupdateon { get; private set; }
        public Int64? cod_produto { get; private set; }
        public Int32? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public Int32? id_tabela { get; private set; }
        public decimal? precovenda { get; private set; }
        public Int64? timestamp { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxProdutosTabelasPrecos() { }

        public LinxProdutosTabelasPrecos(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosTabelasPrecos record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id_tabela = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_tabela);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.cod_produto = ConverterValidatorsExtensions.ConvertToInt64Validation(record.cod_produto);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.precovenda = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.precovenda);
            this.cnpj_emp = record.cnpj_emp;
            this.recordKey = $"[{record.cnpj_emp}]|[{record.cod_produto}]|[{record.id_tabela}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
