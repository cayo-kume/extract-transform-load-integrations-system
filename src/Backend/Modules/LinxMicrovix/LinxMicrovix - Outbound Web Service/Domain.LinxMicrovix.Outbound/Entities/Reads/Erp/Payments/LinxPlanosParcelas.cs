using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Payments
{
    /// <summary>
    /// LinxPlanosParcelas, tabela linxmicrovix.LinxPlanosParcelas. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_planos_parcelas
    /// </summary>
    public class LinxPlanosParcelas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int? plano { get; private set; }
        public int? ordem_parcela { get; private set; }
        public int? prazo_parc { get; private set; }
        public int id_planos_parcelas { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxPlanosParcelas() { }

        public LinxPlanosParcelas(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Payments.LinxPlanosParcelas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            plano = MicrovixConverters.ToNullableInt32(dto.plano);
            ordem_parcela = MicrovixConverters.ToNullableInt32(dto.ordem_parcela);
            prazo_parc = MicrovixConverters.ToNullableInt32(dto.prazo_parc);
            id_planos_parcelas = MicrovixConverters.ToInt32(dto.id_planos_parcelas);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
