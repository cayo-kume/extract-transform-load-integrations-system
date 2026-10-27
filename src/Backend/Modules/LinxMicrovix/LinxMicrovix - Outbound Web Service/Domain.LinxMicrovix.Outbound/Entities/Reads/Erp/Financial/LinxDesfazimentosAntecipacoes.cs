using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxDesfazimentosAntecipacoes, tabela linxmicrovix.LinxDesfazimentosAntecipacoes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_desfazimentos_antecipacoes (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxDesfazimentosAntecipacoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_desfazimentos_antecipacoes { get; private set; }
        public int? id_antecipacoes_financeiras { get; private set; }
        public DateTime? data_desfazimento { get; private set; }
        public string? valor_desfazimento { get; private set; }
        public decimal? valor_acerto { get; private set; }
        public int? tipo_acerto { get; private set; }
        public string? observacoes { get; private set; }
        public int? usuario { get; private set; }
        public long timestamp { get; private set; }

        public LinxDesfazimentosAntecipacoes() { }

        public LinxDesfazimentosAntecipacoes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxDesfazimentosAntecipacoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_desfazimentos_antecipacoes = MicrovixConverters.ToInt32(dto.id_desfazimentos_antecipacoes);
            id_antecipacoes_financeiras = MicrovixConverters.ToNullableInt32(dto.id_antecipacoes_financeiras);
            data_desfazimento = MicrovixConverters.ToNullableDateTime(dto.data_desfazimento);
            valor_desfazimento = MicrovixConverters.ToNullableString(dto.valor_desfazimento, 1000);   //fora da documentação
            valor_acerto = MicrovixConverters.ToNullableDecimal(dto.valor_acerto);
            tipo_acerto = MicrovixConverters.ToNullableInt32(dto.tipo_acerto);
            observacoes = String.IsNullOrEmpty(dto.observacoes) ? null : dto.observacoes;
            usuario = MicrovixConverters.ToNullableInt32(dto.usuario);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
