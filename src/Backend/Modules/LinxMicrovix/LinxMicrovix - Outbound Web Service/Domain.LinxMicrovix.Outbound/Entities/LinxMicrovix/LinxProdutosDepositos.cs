
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxProdutosDepositos
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? portal { get; private set; }
        public Int32? cod_deposito { get; private set; }
        public string? nome_deposito { get; private set; }
        public bool? disponivel { get; private set; }
        public bool? disponivel_transferencia { get; private set; }
        public Int64? timestamp { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxProdutosDepositos() { }

        public LinxProdutosDepositos(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosDepositos record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.cod_deposito = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_deposito);
            this.disponivel = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.disponivel);
            this.disponivel_transferencia = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.disponivel_transferencia);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.nome_deposito = record.nome_deposito;
            this.recordKey = $"[{record.cod_deposito}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
