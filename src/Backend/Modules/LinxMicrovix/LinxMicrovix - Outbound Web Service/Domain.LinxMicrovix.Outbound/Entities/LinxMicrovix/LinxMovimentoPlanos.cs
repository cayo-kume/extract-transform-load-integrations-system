
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxMovimentoPlanos
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public Guid? identificador { get; private set; }
        public Int32? plano { get; private set; }
        public string? desc_plano { get; private set; }
        public decimal? total { get; private set; }
        public Int32? qtde_parcelas { get; private set; }
        public decimal? indice_plano { get; private set; }
        public Int32? cod_forma_pgto { get; private set; }
        public string? forma_pgto { get; private set; }
        public string? tipo_transacao { get; private set; }
        public decimal? taxa_financeira { get; private set; }
        public Int32? ordem_cartao { get; private set; }
        public Int64? timestamp { get; private set; }
        public Int32? empresa { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxMovimentoPlanos() { }

        public LinxMovimentoPlanos(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoPlanos record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.plano = ConverterValidatorsExtensions.ConvertToInt32Validation(record.plano);
            this.qtde_parcelas = ConverterValidatorsExtensions.ConvertToInt32Validation(record.qtde_parcelas);
            this.cod_forma_pgto = ConverterValidatorsExtensions.ConvertToInt32Validation(record.cod_forma_pgto);
            this.ordem_cartao = ConverterValidatorsExtensions.ConvertToInt32Validation(record.ordem_cartao);
            this.empresa = ConverterValidatorsExtensions.ConvertToInt32Validation(record.empresa);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.total = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.total);
            this.indice_plano = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.indice_plano);
            this.taxa_financeira = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.taxa_financeira);
            this.identificador = Guid.Parse(record.identificador);
            this.forma_pgto = record.forma_pgto;
            this.tipo_transacao = record.tipo_transacao;
            this.desc_plano = record.desc_plano;
            this.cnpj_emp = record.cnpj_emp;
            this.recordKey = $"[{record.cnpj_emp}]|[{record.plano}]|[{record.identificador}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
