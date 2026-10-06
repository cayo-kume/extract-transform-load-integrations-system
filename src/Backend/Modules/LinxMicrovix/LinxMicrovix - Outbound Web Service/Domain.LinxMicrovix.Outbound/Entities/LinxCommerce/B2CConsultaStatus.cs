using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce
{
    public class B2CConsultaStatus
    {
        public DateTime? lastupdateon { get; set; }
        public Int32? id_status { get; set; }
        public string? descricao_status { get; set; }
        public Int64? timestamp { get; set; }
        public Int32? portal { get; set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public B2CConsultaStatus() { }

        public B2CConsultaStatus(
            Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaStatus record,
            string? recordXml
        )
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id_status = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_status);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);

            this.descricao_status = record.descricao_status;
            this.recordKey = $"[{record.id_status}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
