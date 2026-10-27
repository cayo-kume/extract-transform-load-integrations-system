using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxDevolucaoRemanejoFabricaStatus, tabela linxmicrovix.LinxDevolucaoRemanejoFabricaStatus. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_devolucao_remanejo_fabrica_status (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxDevolucaoRemanejoFabricaStatus
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int id_devolucao_remanejo_fabrica_status { get; private set; }
        public int? id_devolucao_remanejo_fabrica_tipo { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxDevolucaoRemanejoFabricaStatus() { }

        public LinxDevolucaoRemanejoFabricaStatus(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxDevolucaoRemanejoFabricaStatus dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_devolucao_remanejo_fabrica_status = MicrovixConverters.ToInt32(dto.id_devolucao_remanejo_fabrica_status);
            id_devolucao_remanejo_fabrica_tipo = MicrovixConverters.ToNullableInt32(dto.id_devolucao_remanejo_fabrica_tipo);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
