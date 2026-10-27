using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxExtratoBancarioConciliacoes, tabela linxmicrovix.LinxExtratoBancarioConciliacoes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_extrato_bancario_conciliacoes
    /// </summary>
    public class LinxExtratoBancarioConciliacoes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_extrato_bancario_conciliacoes { get; private set; }
        public int? id_extrato_bancario_lancamentos { get; private set; }
        public long? codigo_fatura { get; private set; }
        public string? dados_anteriores_baixa_fatura { get; private set; }
        public long timestamp { get; private set; }

        public LinxExtratoBancarioConciliacoes() { }

        public LinxExtratoBancarioConciliacoes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxExtratoBancarioConciliacoes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_extrato_bancario_conciliacoes = MicrovixConverters.ToInt32(dto.id_extrato_bancario_conciliacoes);
            id_extrato_bancario_lancamentos = MicrovixConverters.ToNullableInt32(dto.id_extrato_bancario_lancamentos);
            codigo_fatura = MicrovixConverters.ToNullableInt64(dto.codigo_fatura);
            dados_anteriores_baixa_fatura = String.IsNullOrEmpty(dto.dados_anteriores_baixa_fatura) ? null : dto.dados_anteriores_baixa_fatura;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
