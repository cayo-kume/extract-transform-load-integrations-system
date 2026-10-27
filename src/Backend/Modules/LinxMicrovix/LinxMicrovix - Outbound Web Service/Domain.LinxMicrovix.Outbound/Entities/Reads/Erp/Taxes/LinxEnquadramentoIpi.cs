using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxEnquadramentoIpi, tabela linxmicrovix.LinxEnquadramentoIpi. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_enquadramento_ipi
    /// </summary>
    public class LinxEnquadramentoIpi
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_enquadramento_ipi { get; private set; }
        public string? codigo { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }

        public LinxEnquadramentoIpi() { }

        public LinxEnquadramentoIpi(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxEnquadramentoIpi dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_enquadramento_ipi = MicrovixConverters.ToInt32(dto.id_enquadramento_ipi);
            codigo = MicrovixConverters.ToNullableString(dto.codigo, 3);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 600);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
