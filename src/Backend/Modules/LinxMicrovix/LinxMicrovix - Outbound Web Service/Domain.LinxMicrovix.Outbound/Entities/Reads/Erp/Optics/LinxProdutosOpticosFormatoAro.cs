using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Optics
{
    /// <summary>
    /// LinxProdutosOpticosFormatoAro, tabela linxmicrovix.LinxProdutosOpticosFormatoAro. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_formato_aro (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxProdutosOpticosFormatoAro
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_formato_aro { get; private set; }
        public string? codigo { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxProdutosOpticosFormatoAro() { }

        public LinxProdutosOpticosFormatoAro(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Optics.LinxProdutosOpticosFormatoAro dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_formato_aro = MicrovixConverters.ToInt32(dto.id_formato_aro);
            codigo = MicrovixConverters.ToNullableString(dto.codigo, 16);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 60);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
