
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxMovimentoCartoes
    {
        public DateTime? lastupdateon { get; private set; }
        public Guid? identificador { get; private set; }
        public Int32? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? codlojasitef { get; private set; }
        public DateTime? data_lancamento { get; private set; }
        public string? cupomfiscal { get; private set; }
        public string? credito_debito { get; private set; }
        public Int32? id_cartao_bandeira { get; private set; }
        public string? descricao_bandeira { get; private set; }
        public decimal? valor { get; private set; }
        public Int32? ordem_cartao { get; private set; }
        public string? nsu_host { get; private set; }
        public string? nsu_sitef { get; private set; }
        public string? cod_autorizacao { get; private set; }
        public Int32? id_antecipacoes_financeiras { get; private set; }
        public bool? transacao_servico_terceiro { get; private set; }
        public string? texto_comprovante { get; private set; }
        public Int32? id_maquineta_pos { get; private set; }
        public string? descricao_maquineta { get; private set; }
        public string? serie_maquineta { get; private set; }
        public Int64? timestamp { get; private set; }
        public string? cartao_prepago { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxMovimentoCartoes() { }

        public LinxMovimentoCartoes(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxMovimentoCartoes record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.id_cartao_bandeira = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_cartao_bandeira);
            this.ordem_cartao = ConverterValidatorsExtensions.ConvertToInt32Validation(record.ordem_cartao);
            this.id_antecipacoes_financeiras = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_antecipacoes_financeiras);
            this.id_maquineta_pos = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_maquineta_pos);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.data_lancamento = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_lancamento);
            this.identificador = Guid.Parse(record.identificador);
            this.valor = ConverterValidatorsExtensions.ConvertToDecimalValidation(record.valor);
            this.transacao_servico_terceiro = ConverterValidatorsExtensions.ConvertToBooleanValidation(record.transacao_servico_terceiro);
            this.cnpj_emp = record.cnpj_emp;
            this.codlojasitef = record.codlojasitef;
            this.cupomfiscal = record.cupomfiscal;
            this.credito_debito = record.credito_debito;
            this.descricao_bandeira = record.descricao_bandeira;
            this.nsu_host = record.nsu_host;
            this.nsu_sitef = record.nsu_sitef;
            this.cod_autorizacao = record.cod_autorizacao;
            this.texto_comprovante = record.texto_comprovante;
            this.descricao_maquineta = record.descricao_maquineta;
            this.serie_maquineta = record.serie_maquineta;
            this.cartao_prepago = record.cartao_prepago;
            this.recordKey = $"[{record.cnpj_emp}]|[{record.cupomfiscal}]|[{record.cod_autorizacao}]|[{record.identificador}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
