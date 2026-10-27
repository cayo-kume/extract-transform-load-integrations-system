using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Optics
{
    /// <summary>
    /// LinxOticoPrismaDescricao, tabela linxmicrovix.LinxOticoPrismaDescricao. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_otico_prisma_descricao (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxOticoPrismaDescricao
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_otico_prisma_descricao { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxOticoPrismaDescricao() { }

        public LinxOticoPrismaDescricao(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Optics.LinxOticoPrismaDescricao dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_otico_prisma_descricao = MicrovixConverters.ToInt32(dto.id_otico_prisma_descricao);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 10);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
