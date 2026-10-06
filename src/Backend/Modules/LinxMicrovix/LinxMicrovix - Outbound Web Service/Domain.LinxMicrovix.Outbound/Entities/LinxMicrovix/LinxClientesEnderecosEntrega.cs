using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxClientesEnderecosEntrega
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? id_endereco_entrega { get; private set; }
        public Int32? cod_cliente { get; private set; }
        public string? endereco_cliente { get; private set; }
        public string? numero_rua_cliente { get; private set; }
        public string? complemento_end_cli { get; private set; }
        public string? cep_cliente { get; private set; }
        public string? bairro_cliente { get; private set; }
        public string? cidade_cliente { get; private set; }
        public string? uf_cliente { get; private set; }
        public string? descricao { get; private set; }
        public bool? principal { get; private set; }
        public string? fone_cliente { get; private set; }
        public string? fone_celular { get; private set; }
        public Int64? timestamp { get; private set; }
        public Int32? portal { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxClientesEnderecosEntrega() { }

        public LinxClientesEnderecosEntrega(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxClientesEnderecosEntrega record, string recordXml)
        {
            this.lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id_endereco_entrega = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_endereco_entrega);
            this.cod_cliente = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_cliente);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.principal = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.principal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.endereco_cliente = record.endereco_cliente;
            this.numero_rua_cliente = record.numero_rua_cliente;
            this.complemento_end_cli = record.complemento_end_cli;
            this.cep_cliente = record.cep_cliente;
            this.bairro_cliente = record.bairro_cliente;
            this.cidade_cliente = record.cidade_cliente;
            this.uf_cliente = record.uf_cliente;
            this.descricao = record.descricao;
            this.fone_celular = record.fone_celular;
            this.fone_cliente = record.fone_cliente;
            this.recordKey = $"[{record.id_endereco_entrega}]|[{record.cod_cliente}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
