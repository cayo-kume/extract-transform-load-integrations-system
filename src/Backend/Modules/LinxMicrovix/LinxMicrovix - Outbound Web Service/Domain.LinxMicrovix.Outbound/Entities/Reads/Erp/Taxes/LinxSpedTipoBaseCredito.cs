using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxSpedTipoBaseCredito, tabela linxmicrovix.LinxSpedTipoBaseCredito. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_sped_tipo_base_credito (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxSpedTipoBaseCredito
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_sped_tipo_base_credito { get; private set; }
        public string? codigo_sped_tipo_base_credito { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }

        public LinxSpedTipoBaseCredito() { }

        public LinxSpedTipoBaseCredito(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxSpedTipoBaseCredito dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_sped_tipo_base_credito = MicrovixConverters.ToInt32(dto.id_sped_tipo_base_credito);
            codigo_sped_tipo_base_credito = MicrovixConverters.ToNullableString(dto.codigo_sped_tipo_base_credito, 2);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 150);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
