
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxRotinaOrigem
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? codigo_rotina { get; private set; }
        public Int32? portal { get; private set; }
        public string? descricao_rotina { get; private set; }
        public Int64? timestamp { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxRotinaOrigem() { }

        public LinxRotinaOrigem(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxRotinaOrigem record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.codigo_rotina = ConverterValidatorsExtensions.ConvertToInt32Validation(record.codigo_rotina);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.descricao_rotina = record.descricao_rotina;
            this.recordKey = $"[{record.codigo_rotina}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
