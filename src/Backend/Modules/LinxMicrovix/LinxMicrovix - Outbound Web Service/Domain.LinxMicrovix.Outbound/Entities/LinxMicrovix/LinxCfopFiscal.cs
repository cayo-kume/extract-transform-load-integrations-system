
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxCfopFiscal
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32 portal { get; private set; }
        public Int32 id_cfop_fiscal { get; private set; }
        public string? cfop_fiscal { get; private set; }
        public string? descricao { get; private set; }
        public bool excluido { get; private set; }
        public Int64 timestamp { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxCfopFiscal() { }

        public LinxCfopFiscal(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxCfopFiscal record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id_cfop_fiscal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_cfop_fiscal);
            this.excluido = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.excluido);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.cfop_fiscal = record.cfop_fiscal;
            this.descricao = record.descricao;
            this.recordKey = $"[{record.id_cfop_fiscal}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
