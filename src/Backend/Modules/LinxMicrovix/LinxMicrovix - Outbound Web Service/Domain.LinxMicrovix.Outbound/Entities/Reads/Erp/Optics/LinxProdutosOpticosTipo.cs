using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Optics
{
    /// <summary>
    /// LinxProdutosOpticosTipo, tabela linxmicrovix.LinxProdutosOpticosTipo. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_produtos_opticos_tipo (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxProdutosOpticosTipo
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_produtos_opticos_tipo { get; private set; }
        public string? tipo { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxProdutosOpticosTipo() { }

        public LinxProdutosOpticosTipo(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Optics.LinxProdutosOpticosTipo dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_produtos_opticos_tipo = MicrovixConverters.ToInt32(dto.id_produtos_opticos_tipo);
            tipo = MicrovixConverters.ToNullableString(dto.tipo, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
