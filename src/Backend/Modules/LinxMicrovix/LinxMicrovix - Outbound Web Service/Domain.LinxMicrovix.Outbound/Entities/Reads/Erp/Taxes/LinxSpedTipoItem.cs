using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Taxes
{
    /// <summary>
    /// LinxSpedTipoItem, tabela linxmicrovix.LinxSpedTipoItem. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_sped_tipo_item (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxSpedTipoItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_sped_tipo_item { get; private set; }
        public string? codigo { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }

        public LinxSpedTipoItem() { }

        public LinxSpedTipoItem(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Taxes.LinxSpedTipoItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_sped_tipo_item = MicrovixConverters.ToInt32(dto.id_sped_tipo_item);
            codigo = MicrovixConverters.ToNullableString(dto.codigo, 2);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 60);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
