
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxSetores
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? id_setor { get; private set; }
        public string? desc_setor { get; private set; }
        public Int64? timestamp { get; private set; }
        public string? codigo_integracao_ws { get; private set; }
        public Int32? ativo { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxSetores() { }

        public LinxSetores(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxSetores record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id_setor = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_setor);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.ativo = ConverterValidatorsExtensions.ConvertToInt32Validation(record.ativo);
            this.desc_setor = record.desc_setor;
            this.recordKey = $"[{record.id_setor}]|[{record.timestamp}]";
            this.recordXml = recordXml;
            this.codigo_integracao_ws = record.codigo_integracao_ws;
        }
    }
}
