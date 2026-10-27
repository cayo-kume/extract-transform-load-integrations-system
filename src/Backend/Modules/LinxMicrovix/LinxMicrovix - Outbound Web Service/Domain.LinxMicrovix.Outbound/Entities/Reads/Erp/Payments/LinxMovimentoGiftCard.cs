using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Payments
{
    /// <summary>
    /// LinxMovimentoGiftCard, tabela linxmicrovix.LinxMovimentoGiftCard. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + identificador_movimento + nsu_cliente (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxMovimentoGiftCard
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? empresa { get; private set; }
        public DateTime? data_transacao { get; private set; }
        public int? operacao { get; private set; }
        public string nsu_cliente { get; private set; } = String.Empty;
        public string? nsu_host { get; private set; }
        public decimal? valor { get; private set; }
        public string? json_envio { get; private set; }
        public string? json_retorno { get; private set; }
        public int? qtde_tentativa { get; private set; }
        public string? aprovado_barramento { get; private set; }
        public bool? estornada { get; private set; }
        public string? codigo_loja { get; private set; }
        public Guid identificador_movimento { get; private set; }
        public string? numero_cartao { get; private set; }
        public int? plano { get; private set; }
        public bool? ambiente_producao { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxMovimentoGiftCard() { }

        public LinxMovimentoGiftCard(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Payments.LinxMovimentoGiftCard dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            data_transacao = MicrovixConverters.ToNullableDateTime(dto.data_transacao);
            operacao = MicrovixConverters.ToNullableInt32(dto.operacao);
            nsu_cliente = MicrovixConverters.ToNullableString(dto.nsu_cliente, 100) ?? String.Empty;
            nsu_host = MicrovixConverters.ToNullableString(dto.nsu_host, 100);
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            json_envio = String.IsNullOrEmpty(dto.json_envio) ? null : dto.json_envio;
            json_retorno = String.IsNullOrEmpty(dto.json_retorno) ? null : dto.json_retorno;
            qtde_tentativa = MicrovixConverters.ToNullableInt32(dto.qtde_tentativa);
            aprovado_barramento = MicrovixConverters.ToNullableString(dto.aprovado_barramento, 4);
            estornada = MicrovixConverters.ToNullableBoolean(dto.estornada);
            codigo_loja = MicrovixConverters.ToNullableString(dto.codigo_loja, 100);
            identificador_movimento = MicrovixConverters.ToNullableGuid(dto.identificador_movimento) ?? Guid.Empty;
            numero_cartao = MicrovixConverters.ToNullableString(dto.numero_cartao, 100);
            plano = MicrovixConverters.ToNullableInt32(dto.plano);
            ambiente_producao = MicrovixConverters.ToNullableBoolean(dto.ambiente_producao);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
