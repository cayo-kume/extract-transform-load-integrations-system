using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxAntecipacoesFinanceirasPlanos, tabela linxmicrovix.LinxAntecipacoesFinanceirasPlanos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_antecipacoes_financeiras + plano + forma_pgto (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxAntecipacoesFinanceirasPlanos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public int id_antecipacoes_financeiras { get; private set; }
        public int? numero_antecipacao { get; private set; }
        public string forma_pgto { get; private set; } = String.Empty;
        public int plano { get; private set; }
        public string? nome_plano { get; private set; }
        public decimal? valor { get; private set; }
        public bool? cancelado { get; private set; }
        public long timestamp { get; private set; }
        public DateTime? data_criacao_antecipacao_plano { get; private set; }

        public LinxAntecipacoesFinanceirasPlanos() { }

        public LinxAntecipacoesFinanceirasPlanos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxAntecipacoesFinanceirasPlanos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_antecipacoes_financeiras = MicrovixConverters.ToInt32(dto.id_antecipacoes_financeiras);
            numero_antecipacao = MicrovixConverters.ToNullableInt32(dto.numero_antecipacao);
            forma_pgto = MicrovixConverters.ToNullableString(dto.forma_pgto, 50) ?? String.Empty;
            plano = MicrovixConverters.ToInt32(dto.plano);
            nome_plano = MicrovixConverters.ToNullableString(dto.nome_plano, 100);
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            cancelado = MicrovixConverters.ToNullableBoolean(dto.cancelado);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            data_criacao_antecipacao_plano = MicrovixConverters.ToNullableDateTime(dto.data_criacao_antecipacao_plano);
        }
    }
}
