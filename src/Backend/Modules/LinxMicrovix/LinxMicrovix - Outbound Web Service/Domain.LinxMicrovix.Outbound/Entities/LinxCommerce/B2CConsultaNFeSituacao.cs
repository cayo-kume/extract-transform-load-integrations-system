
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce
{
    public class B2CConsultaNFeSituacao
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? id_nfe_situacao { get; private set; }
        public string? descricao { get; private set; }
        public Int64? timestamp { get; private set; }
        public Int32? portal { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public B2CConsultaNFeSituacao() { }

        public B2CConsultaNFeSituacao(
            Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaNFeSituacao record,
            string? recordXml
        )
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id_nfe_situacao = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_nfe_situacao);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);

            this.descricao = record.descricao;
            this.recordKey = $"[{record.id_nfe_situacao}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
