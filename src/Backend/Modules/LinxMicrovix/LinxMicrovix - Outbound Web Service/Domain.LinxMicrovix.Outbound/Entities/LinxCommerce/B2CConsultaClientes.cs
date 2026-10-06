
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce
{
    public class B2CConsultaClientes
    {
        public DateTime? lastupdateon { get; private set; }
        public int? cod_cliente_b2c { get; private set; }
        public int? cod_cliente_erp { get; private set; }
        public string? doc_cliente { get; private set; }
        public string? nm_cliente { get; private set; }
        public string? nm_mae { get; private set; }
        public string? nm_pai { get; private set; }
        public string? nm_conjuge { get; private set; }
        public DateTime? dt_cadastro { get; private set; }
        public DateTime? dt_nasc_cliente { get; private set; }
        public string? end_cliente { get; private set; }
        public string? complemento_end_cliente { get; private set; }
        public string? nr_rua_cliente { get; private set; }
        public string? bairro_cliente { get; private set; }
        public string? cep_cliente { get; private set; }
        public string? cidade_cliente { get; private set; }
        public string? uf_cliente { get; private set; }
        public string? fone_cliente { get; private set; }
        public string? fone_comercial { get; private set; }
        public string? cel_cliente { get; private set; }
        public string? email_cliente { get; private set; }
        public string? rg_cliente { get; private set; }
        public string? rg_orgao_emissor { get; private set; }
        public Int32? estado_civil_cliente { get; private set; }
        public string? empresa_cliente { get; private set; }
        public string? cargo_cliente { get; private set; }
        public string? sexo_cliente { get; private set; }
        public DateTime? dt_update { get; private set; }
        public Int32? ativo { get; private set; }
        public bool? receber_email { get; private set; }
        public DateTime? dt_expedicao_rg { get; private set; }
        public string? naturalidade { get; private set; }
        public Int32? tempo_residencia { get; private set; }
        public decimal? renda { get; private set; }
        public string? numero_compl_rua_cliente { get; private set; }
        public Int64? timestamp { get; private set; }
        public string? tipo_pessoa { get; private set; }
        public Int32? portal { get; private set; }
        public bool? aceita_programa_fidelidade { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public B2CConsultaClientes() { }

        public B2CConsultaClientes(
            Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce.B2CConsultaClientes record,
            string? recordXml
        )
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.cod_cliente_b2c = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_cliente_b2c);
            this.cod_cliente_erp = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_cliente_erp);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.estado_civil_cliente = ConverterValidatorsExtensions.ConvertToInt32Validation(record.estado_civil_cliente);
            this.ativo = ConverterValidatorsExtensions.ConvertToInt32Validation(record.ativo);
            this.tempo_residencia = ConverterValidatorsExtensions.ConvertToInt32Validation(record.tempo_residencia);
            this.renda = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.renda);
            this.receber_email = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.receber_email);
            this.aceita_programa_fidelidade = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.aceita_programa_fidelidade);
            this.dt_cadastro = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_cadastro);
            this.dt_expedicao_rg = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_expedicao_rg);
            this.dt_update = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_update);
            this.dt_nasc_cliente = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_nasc_cliente);

            //Refatorar Aqui (adicionar operador ternario para substring)
            this.doc_cliente = record.doc_cliente;
            this.nm_cliente = record.nm_cliente;
            this.nm_mae = record.nm_mae;
            this.nm_pai = record.nm_pai;
            this.nm_conjuge = record.nm_conjuge;
            this.end_cliente = record.end_cliente;
            this.complemento_end_cliente = record.complemento_end_cliente;
            this.nr_rua_cliente = record.nr_rua_cliente;
            this.bairro_cliente = record.bairro_cliente;
            this.cep_cliente = record.cep_cliente;
            this.cidade_cliente = record.cidade_cliente;
            this.uf_cliente = record.uf_cliente;
            this.fone_cliente = record.fone_cliente;
            this.fone_comercial = record.fone_comercial;
            this.cel_cliente = record.cel_cliente;
            this.email_cliente = record.email_cliente;
            this.rg_cliente = record.rg_cliente;
            this.rg_orgao_emissor = record.rg_orgao_emissor;
            this.empresa_cliente = record.empresa_cliente;
            this.cargo_cliente = record.cargo_cliente;
            this.sexo_cliente = record.sexo_cliente;
            this.naturalidade = record.naturalidade;
            this.numero_compl_rua_cliente = record.numero_compl_rua_cliente;
            this.tipo_pessoa = record.tipo_pessoa;
            this.recordKey = $"[{record.cod_cliente_b2c}]|[{record.cod_cliente_erp}]|[{record.doc_cliente}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
