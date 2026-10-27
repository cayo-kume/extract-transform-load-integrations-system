using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Loyalty
{
    /// <summary>
    /// LinxFidelidade, tabela linxmicrovix.LinxFidelidade. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_fidelidade_parceiro_log (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxFidelidade
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_fidelidade_parceiro_log { get; private set; }
        public DateTime? data_transacao { get; private set; }
        public int? operacao { get; private set; }
        public string? aprovado_barramento { get; private set; }
        public decimal? valor_monetario { get; private set; }
        public string? numero_cartao { get; private set; }
        public Guid? identificador_movimento { get; private set; }
        public long timestamp { get; private set; }

        public LinxFidelidade() { }

        public LinxFidelidade(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Loyalty.LinxFidelidade dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_fidelidade_parceiro_log = MicrovixConverters.ToInt32(dto.id_fidelidade_parceiro_log);
            data_transacao = MicrovixConverters.ToNullableDateTime(dto.data_transacao);
            operacao = MicrovixConverters.ToNullableInt32(dto.operacao);
            aprovado_barramento = MicrovixConverters.ToNullableString(dto.aprovado_barramento, 4);
            valor_monetario = MicrovixConverters.ToNullableDecimal(dto.valor_monetario);
            numero_cartao = MicrovixConverters.ToNullableString(dto.numero_cartao, 100);
            identificador_movimento = MicrovixConverters.ToNullableGuid(dto.identificador_movimento);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
