
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxLojas
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? portal { get; private set; }
        public Int32? empresa { get; private set; }
        public string? nome_emp { get; private set; }
        public string? razao_emp { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? inscricao_emp { get; private set; }
        public string? endereco_emp { get; private set; }
        public Int32? num_emp { get; private set; }
        public string? complement_emp { get; private set; }
        public string? bairro_emp { get; private set; }
        public string? cep_emp { get; private set; }
        public string? cidade_emp { get; private set; }
        public string? estado_emp { get; private set; }
        public string? fone_emp { get; private set; }
        public string? email_emp { get; private set; }
        public Int32? cod_ibge_municipio { get; private set; }
        public DateTime? data_criacao_emp { get; private set; }
        public DateTime? data_criacao_portal { get; private set; }
        public string? sistema_tributacao { get; private set; }
        public Int32? regime_tributario { get; private set; }
        public decimal? area_empresa { get; private set; }
        public Int64? timestamp { get; private set; }
        public string? sigla_empresa { get; private set; }
        public Int32? id_classe_fiscal { get; private set; }
        public bool? centro_distribuicao { get; private set; }
        public string? inscricao_municipal_emp { get; private set; }
        public string? cnae_emp { get; private set; }
        public string? cod_cliente_linx { get; private set; }
        public Int32? tabela_preco_preferencial { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxLojas() { }

        public LinxLojas(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxLojas record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.empresa = ConverterValidatorsExtensions.ConvertToInt32Validation(record.empresa);
            this.num_emp = ConverterValidatorsExtensions.ConvertToInt32Validation(record.num_emp);
            this.cod_ibge_municipio = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_ibge_municipio);
            this.regime_tributario = ConverterValidatorsExtensions.ConvertToInt32Validation(record.regime_tributario);
            this.id_classe_fiscal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_classe_fiscal);
            this.tabela_preco_preferencial = ConverterValidatorsExtensions.ConvertToInt32Validation(record.tabela_preco_preferencial);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.area_empresa = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.area_empresa);
            this.centro_distribuicao = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.centro_distribuicao);
            this.data_criacao_emp = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_criacao_emp);
            this.data_criacao_portal = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_criacao_portal);
            this.cidade_emp = record.cidade_emp;
            this.cnae_emp = record.cnae_emp;
            this.inscricao_municipal_emp = record.inscricao_municipal_emp;
            this.sigla_empresa = record.sigla_empresa;
            this.sistema_tributacao = record.sistema_tributacao;
            this.email_emp = record.email_emp;
            this.fone_emp = record.fone_emp;
            this.estado_emp = record.estado_emp;
            this.cep_emp = record.cep_emp;
            this.bairro_emp = record.bairro_emp;
            this.complement_emp = record.complement_emp;
            this.endereco_emp = record.endereco_emp;
            this.inscricao_emp = record.inscricao_emp;
            this.cnae_emp = record.cnae_emp;
            this.razao_emp = record.razao_emp;
            this.nome_emp = record.nome_emp;
            this.cnpj_emp = record.cnpj_emp;
            this.cod_cliente_linx = record.cod_cliente_linx;
            this.recordKey = $"[{record.empresa}]|[{record.cnpj_emp}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
