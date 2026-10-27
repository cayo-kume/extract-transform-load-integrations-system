using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxMovimentoObservacaoCF, tabela linxmicrovix.LinxMovimentoObservacaoCF. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + identificador
    /// </summary>
    public class LinxMovimentoObservacaoCF
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid identificador { get; private set; }
        public string? documento_cliente { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxMovimentoObservacaoCF() { }

        public LinxMovimentoObservacaoCF(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxMovimentoObservacaoCF dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            identificador = MicrovixConverters.ToNullableGuid(dto.identificador) ?? Guid.Empty;
            documento_cliente = MicrovixConverters.ToNullableString(dto.documento_cliente, 14);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
