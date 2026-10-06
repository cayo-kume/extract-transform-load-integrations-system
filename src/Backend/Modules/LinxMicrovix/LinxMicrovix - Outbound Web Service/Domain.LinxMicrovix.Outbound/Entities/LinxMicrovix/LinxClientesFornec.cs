
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxClientesFornec
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? portal { get; private set; }
        public Int32? cod_cliente { get; private set; }
        public string? razao_cliente { get; private set; }
        public string? nome_cliente { get; private set; }
        public string? doc_cliente { get; private set; }
        public string? tipo_cliente { get; private set; }
        public string? endereco_cliente { get; private set; }
        public string? numero_rua_cliente { get; private set; }
        public string? complement_end_cli { get; private set; }
        public string? bairro_cliente { get; private set; }
        public string? cep_cliente { get; private set; }
        public string? cidade_cliente { get; private set; }
        public string? uf_cliente { get; private set; }
        public string? pais { get; private set; }
        public string? fone_cliente { get; private set; }
        public string? email_cliente { get; private set; }
        public string? sexo { get; private set; }
        public DateTime? data_cadastro { get; private set; }
        public DateTime? data_nascimento { get; private set; }
        public string? cel_cliente { get; private set; }
        public string? ativo { get; private set; }
        public DateTime? dt_update { get; private set; }
        public string? inscricao_estadual { get; private set; }
        public string? incricao_municipal { get; private set; }
        public string? identidade_cliente { get; private set; }
        public string? cartao_fidelidade { get; private set; }
        public Int32? cod_ibge_municipio { get; private set; }
        public string? classe_cliente { get; private set; }
        public string? matricula_conveniado { get; private set; }
        public string? tipo_cadastro { get; private set; }
        public Int32? empresa_cadastro { get; private set; }
        public Int32? id_estado_civil { get; private set; }
        public string? fax_cliente { get; private set; }
        public string? site_cliente { get; private set; }
        public Int64? timestamp { get; private set; }
        public bool? cliente_anonimo { get; private set; }
        public decimal? limite_compras { get; private set; }
        public string? codigo_ws { get; private set; }
        public decimal? limite_credito_compra { get; private set; }
        public Int32? id_classe_fiscal { get; private set; }
        public string? obs { get; private set; }
        public string? mae { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxClientesFornec() { }

        public LinxClientesFornec(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxClientesFornec record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.cod_cliente = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_cliente);
            this.cod_ibge_municipio = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_ibge_municipio);
            this.empresa_cadastro = ConverterValidatorsExtensions.ConvertToInt32Validation(record.empresa_cadastro);
            this.id_estado_civil = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_estado_civil);
            this.id_classe_fiscal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_classe_fiscal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.dt_update = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.dt_update);
            this.data_nascimento = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_nascimento);
            this.data_cadastro = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_cadastro);
            this.cliente_anonimo = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.cliente_anonimo);
            this.limite_compras = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.limite_compras);
            this.limite_credito_compra = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.limite_credito_compra);
            this.razao_cliente = record.razao_cliente;
            this.nome_cliente = record.nome_cliente;
            this.doc_cliente = record.doc_cliente;
            this.tipo_cliente = record.tipo_cliente;
            this.endereco_cliente = record.endereco_cliente;
            this.numero_rua_cliente = record.numero_rua_cliente;
            this.complement_end_cli = record.complement_end_cli;
            this.bairro_cliente = record.bairro_cliente;
            this.cep_cliente = record.cep_cliente;
            this.cidade_cliente = record.cidade_cliente;
            this.uf_cliente = record.uf_cliente;
            this.pais = record.pais;
            this.fone_cliente = record.fone_cliente;
            this.email_cliente = record.email_cliente;
            this.sexo = record.sexo;
            this.cel_cliente = record.cel_cliente;
            this.ativo = record.ativo;
            this.incricao_municipal = record.incricao_municipal;
            this.inscricao_estadual = record.inscricao_estadual;
            this.identidade_cliente = record.identidade_cliente;
            this.cartao_fidelidade = record.cartao_fidelidade;
            this.classe_cliente = record.classe_cliente;
            this.matricula_conveniado = record.matricula_conveniado;
            this.tipo_cadastro = record.tipo_cadastro;
            this.fax_cliente = record.fax_cliente;
            this.site_cliente = record.site_cliente;
            this.codigo_ws = record.codigo_ws;
            this.obs = record.obs;
            this.mae = record.mae;
            this.recordKey = $"[{record.cod_cliente}]|[{record.doc_cliente}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
