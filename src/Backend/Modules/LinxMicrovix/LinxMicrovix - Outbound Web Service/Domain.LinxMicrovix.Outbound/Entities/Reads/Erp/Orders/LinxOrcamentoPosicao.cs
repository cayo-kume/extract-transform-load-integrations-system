using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders
{
    /// <summary>
    /// LinxOrcamentoPosicao, tabela linxmicrovix.LinxOrcamentoPosicao. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_posicao (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxOrcamentoPosicao
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_posicao { get; private set; }
        public string? descricao_posicao { get; private set; }
        public string? ativo_posicao { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxOrcamentoPosicao() { }

        public LinxOrcamentoPosicao(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Orders.LinxOrcamentoPosicao dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_posicao = MicrovixConverters.ToInt32(dto.id_posicao);
            descricao_posicao = MicrovixConverters.ToNullableString(dto.descricao_posicao, 50);
            ativo_posicao = MicrovixConverters.ToNullableString(dto.ativo_posicao, 1);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);   //fora da documentação
        }
    }
}
