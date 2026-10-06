
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxVendedores
    {
        public DateTime lastupdateon { get; private set; }
        public Int32? cod_vendedor { get; private set; }
        public string? nome_vendedor { get; private set; }
        public string? tipo_vendedor { get; private set; }
        public string? end_vend_rua { get; private set; }
        public string? end_vend_numero { get; private set; }
        public string? end_vend_complemento { get; private set; }
        public string? end_vend_bairro { get; private set; }
        public string? end_vend_cep { get; private set; }
        public string? end_vend_cidade { get; private set; }
        public string? end_vend_uf { get; private set; }
        public string? fone_vendedor { get; private set; }
        public string? mail_vendedor { get; private set; }
        public DateTime? dt_upd { get; private set; }
        public string? cpf_vendedor { get; private set; }
        public string? ativo { get; private set; }
        public DateTime? data_admissao { get; private set; }
        public DateTime? data_saida { get; private set; }
        public Int32? portal { get; private set; }
        public Int64? timestamp { get; private set; }
        public string? matricula { get; private set; }
        public Int32? id_tipo_venda { get; private set; }
        public string? descricao_tipo_venda { get; private set; }
        public string? cargo { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxVendedores() { }

        public LinxVendedores(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxVendedores record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.cod_vendedor = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_vendedor);
            this.dt_upd = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_upd);
            this.data_admissao = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_admissao);
            this.data_saida = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_saida);
            this.id_tipo_venda = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_tipo_venda);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.nome_vendedor = record.nome_vendedor;
            this.tipo_vendedor = record.tipo_vendedor;
            this.end_vend_rua = record.end_vend_rua;
            this.end_vend_numero = record.end_vend_numero;
            this.end_vend_complemento = record.end_vend_complemento;
            this.end_vend_bairro = record.end_vend_bairro;
            this.end_vend_cep = record.end_vend_cep;
            this.end_vend_cidade = record.end_vend_cidade;
            this.end_vend_uf = record.end_vend_uf;
            this.fone_vendedor = record.fone_vendedor;
            this.mail_vendedor = record.mail_vendedor;
            this.cpf_vendedor = record.cpf_vendedor;
            this.ativo = record.ativo;
            this.matricula = record.matricula;
            this.descricao_tipo_venda = record.descricao_tipo_venda;
            this.cargo = record.cargo;
            this.recordKey = $"[{record.cod_vendedor}]|[{record.cpf_vendedor}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
