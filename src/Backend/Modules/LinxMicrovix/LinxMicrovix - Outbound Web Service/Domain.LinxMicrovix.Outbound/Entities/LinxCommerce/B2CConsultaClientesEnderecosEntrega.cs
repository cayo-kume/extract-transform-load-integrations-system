
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce
{
    public class B2CConsultaClientesEnderecosEntrega
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? id_endereco_entrega { get; private set; }
        public Int32? cod_cliente_erp { get; private set; }
        public Int32? cod_cliente_b2c { get; private set; }
        public string? endereco_cliente { get; private set; }
        public string? numero_rua_cliente { get; private set; }
        public string? complemento_end_cli { get; private set; }
        public string? cep_cliente { get; private set; }
        public string? bairro_cliente { get; private set; }
        public string? cidade_cliente { get; private set; }
        public string? uf_cliente { get; private set; }
        public string? descricao { get; private set; }
        public bool? principal { get; private set; }
        public Int32? id_cidade { get; private set; }
        public Int64? timestamp { get; private set; }
        public Int32? portal { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public B2CConsultaClientesEnderecosEntrega() { }

        public B2CConsultaClientesEnderecosEntrega(
            Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaClientesEnderecosEntrega record,
            string? recordXml
        )
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.id_endereco_entrega = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_endereco_entrega);
            this.cod_cliente_erp = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_cliente_erp);
            this.cod_cliente_b2c = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_cliente_b2c);
            this.principal = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.principal);
            this.id_cidade = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_cidade);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);

            this.endereco_cliente = record.endereco_cliente;
            this.numero_rua_cliente = record.numero_rua_cliente;
            this.complemento_end_cli = record.complemento_end_cli;
            this.cep_cliente = record.cep_cliente;
            this.bairro_cliente = record.bairro_cliente;
            this.cidade_cliente = record.cep_cliente;
            this.uf_cliente = record.uf_cliente;
            this.descricao = record.descricao;
            this.recordKey = $"[{record.id_endereco_entrega}]|[{record.cod_cliente_b2c}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
