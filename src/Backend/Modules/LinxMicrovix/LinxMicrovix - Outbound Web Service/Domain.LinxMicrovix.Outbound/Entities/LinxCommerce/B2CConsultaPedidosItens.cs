
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce
{
    public class B2CConsultaPedidosItens
    {
        public DateTime? lastupdateon { get; private set; }
        public Int64? id_pedido_item { get; private set; }
        public Int64? id_pedido { get; private set; }
        public Int64? codigoproduto { get; private set; }
        public Int32? quantidade { get; private set; }
        public decimal? vl_unitario { get; private set; }
        public Int64? timestamp { get; private set; }
        public Int32? portal { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public B2CConsultaPedidosItens() { }

        public B2CConsultaPedidosItens(
            Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaPedidosItens record,
            string? recordXml
        )
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id_pedido_item = ConverterValidatorsExtensions.ConvertToInt64Validation(record.id_pedido_item);
            this.id_pedido = ConverterValidatorsExtensions.ConvertToInt64Validation(record.id_pedido);
            this.codigoproduto = ConverterValidatorsExtensions.ConvertToInt64Validation(record.codigoproduto);
            this.quantidade = ConverterValidatorsExtensions.ConvertToInt32Validation(record.quantidade);
            this.vl_unitario = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.vl_unitario);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.recordKey = $"[{record.id_pedido_item}]|[{record.id_pedido}]|[{record.codigoproduto}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
