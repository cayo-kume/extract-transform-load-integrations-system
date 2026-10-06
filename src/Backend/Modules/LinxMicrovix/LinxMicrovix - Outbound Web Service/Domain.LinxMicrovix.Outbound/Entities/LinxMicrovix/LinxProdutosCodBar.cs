
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxProdutosCodBar
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? portal { get; set; }
        public Int64? cod_produto { get; set; }
        public string? cod_barra { get; set; }
        public Int64? timestamp { get; set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxProdutosCodBar() { }

        public LinxProdutosCodBar(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosCodBar record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.cod_produto = ConverterValidatorsExtensions.ConvertToInt64Validation(record.cod_produto);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.cod_barra = record.cod_barra;
            this.recordKey = $"[{record.cod_produto}]|[{record.cod_barra}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
