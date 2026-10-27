using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Optics
{
    /// <summary>
    /// LinxProdutosOpticosTipoAro, tabela linxmicrovix.LinxProdutosOpticosTipoAro. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_tipo_aro (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxProdutosOpticosTipoAro
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_tipo_aro { get; private set; }
        public string? tipo_aro { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxProdutosOpticosTipoAro() { }

        public LinxProdutosOpticosTipoAro(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Optics.LinxProdutosOpticosTipoAro dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_tipo_aro = MicrovixConverters.ToInt32(dto.id_tipo_aro);
            tipo_aro = MicrovixConverters.ToNullableString(dto.tipo_aro, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
