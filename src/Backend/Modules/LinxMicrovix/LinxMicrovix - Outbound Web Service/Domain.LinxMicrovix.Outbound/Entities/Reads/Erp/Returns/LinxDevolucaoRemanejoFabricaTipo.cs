using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxDevolucaoRemanejoFabricaTipo, tabela linxmicrovix.LinxDevolucaoRemanejoFabricaTipo. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_devolucao_remanejo_fabrica_tipo (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxDevolucaoRemanejoFabricaTipo
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_devolucao_remanejo_fabrica_tipo { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxDevolucaoRemanejoFabricaTipo() { }

        public LinxDevolucaoRemanejoFabricaTipo(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxDevolucaoRemanejoFabricaTipo dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_devolucao_remanejo_fabrica_tipo = MicrovixConverters.ToInt32(dto.id_devolucao_remanejo_fabrica_tipo);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
