using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Prices
{
    /// <summary>
    /// LinxMotivoDesconto, tabela linxmicrovix.LinxMotivoDesconto. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_motivo_desconto (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxMotivoDesconto
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_motivo_desconto { get; private set; }
        public string? desc_motivo_desconto { get; private set; }
        public bool? ativo { get; private set; }
        public bool? excluido { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxMotivoDesconto() { }

        public LinxMotivoDesconto(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Prices.LinxMotivoDesconto dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_motivo_desconto = MicrovixConverters.ToInt32(dto.id_motivo_desconto);
            desc_motivo_desconto = MicrovixConverters.ToNullableString(dto.desc_motivo_desconto, 60);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
