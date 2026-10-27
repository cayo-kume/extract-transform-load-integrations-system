using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Payments
{
    /// <summary>
    /// LinxMovimentoCartoes, tabela linxmicrovix.LinxMovimentoCartoes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador + ordem_cartao
    /// </summary>
    public class LinxMovimentoCartoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public string? codlojasitef { get; private set; }
        public DateTime? data_lancamento { get; private set; }
        public Guid identificador { get; private set; }
        public string? cupomfiscal { get; private set; }
        public string? credito_debito { get; private set; }
        public int? id_cartao_bandeira { get; private set; }
        public string? descricao_bandeira { get; private set; }
        public decimal? valor { get; private set; }
        public int ordem_cartao { get; private set; }
        public string? nsu_host { get; private set; }
        public string? nsu_sitef { get; private set; }
        public string? cod_autorizacao { get; private set; }
        public int? id_antecipacoes_financeiras { get; private set; }
        public bool? transacao_servico_terceiro { get; private set; }
        public string? texto_comprovante { get; private set; }
        public int? id_maquineta_pos { get; private set; }
        public string? descricao_maquineta { get; private set; }
        public string? serie_maquineta { get; private set; }
        public long timestamp { get; private set; }
        public string? cartao_prepago { get; private set; }
        public bool? transacao_cancelada_smartpos { get; private set; }
        public int? usuario_cancelamento_transacao_smartpos { get; private set; }
        public DateTime? data_hora_cancelamento_transacao_smartpos { get; private set; }

        public LinxMovimentoCartoes() { }

        public LinxMovimentoCartoes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Payments.LinxMovimentoCartoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            codlojasitef = MicrovixConverters.ToNullableString(dto.codlojasitef, 10);
            data_lancamento = MicrovixConverters.ToNullableDateTime(dto.data_lancamento);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador) ?? Guid.Empty;
            cupomfiscal = MicrovixConverters.ToNullableString(dto.cupomfiscal, 20);
            credito_debito = MicrovixConverters.ToNullableString(dto.credito_debito, 1);
            id_cartao_bandeira = MicrovixConverters.ToNullableInt32(dto.id_cartao_bandeira);
            descricao_bandeira = MicrovixConverters.ToNullableString(dto.descricao_bandeira, 100);
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            ordem_cartao = MicrovixConverters.ToInt32(dto.ordem_cartao);
            nsu_host = MicrovixConverters.ToNullableString(dto.nsu_host, 50);
            nsu_sitef = MicrovixConverters.ToNullableString(dto.nsu_sitef, 50);
            cod_autorizacao = MicrovixConverters.ToNullableString(dto.cod_autorizacao, 50);
            id_antecipacoes_financeiras = MicrovixConverters.ToNullableInt32(dto.id_antecipacoes_financeiras);
            transacao_servico_terceiro = MicrovixConverters.ToNullableBoolean(dto.transacao_servico_terceiro);   //fora da documentação
            texto_comprovante = String.IsNullOrEmpty(dto.texto_comprovante) ? null : dto.texto_comprovante;
            id_maquineta_pos = MicrovixConverters.ToNullableInt32(dto.id_maquineta_pos);
            descricao_maquineta = MicrovixConverters.ToNullableString(dto.descricao_maquineta, 50);
            serie_maquineta = MicrovixConverters.ToNullableString(dto.serie_maquineta, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            cartao_prepago = MicrovixConverters.ToNullableString(dto.cartao_prepago, 1);
            transacao_cancelada_smartpos = MicrovixConverters.ToNullableBoolean(dto.transacao_cancelada_smartpos);
            usuario_cancelamento_transacao_smartpos = MicrovixConverters.ToNullableInt32(dto.usuario_cancelamento_transacao_smartpos);
            data_hora_cancelamento_transacao_smartpos = MicrovixConverters.ToNullableDateTime(dto.data_hora_cancelamento_transacao_smartpos);
        }
    }
}
