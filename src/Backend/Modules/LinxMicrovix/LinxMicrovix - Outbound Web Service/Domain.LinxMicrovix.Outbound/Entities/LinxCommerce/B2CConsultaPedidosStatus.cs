
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce
{
    public class B2CConsultaPedidosStatus
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? id { get; private set; }
        public Int32? id_status { get; private set; }
        public Int32? id_pedido { get; private set; }
        public DateTime? data_hora { get; private set; }
        public string? anotacao { get; private set; }
        public Int64? timestamp { get; private set; }
        public Int32? portal { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public B2CConsultaPedidosStatus() { }

        public B2CConsultaPedidosStatus(
            Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosStatus record,
            string? recordXml
        )
        {
            lastupdateon = ConverterValidatorsExtensions.ConvertToDateTimeValidation<DateTime>(DateTime.Now);
            this.id = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id);
            this.id_status = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_status);
            this.id_pedido = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_pedido);
            this.data_hora = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_hora);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);

            this.anotacao = record.anotacao;
            this.recordKey = $"[{record.id}]|[{record.id_status}]|[{record.id_pedido}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
