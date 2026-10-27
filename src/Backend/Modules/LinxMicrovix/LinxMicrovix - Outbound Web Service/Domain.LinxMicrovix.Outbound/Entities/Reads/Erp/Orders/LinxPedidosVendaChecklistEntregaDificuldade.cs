using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders
{
    /// <summary>
    /// LinxPedidosVendaChecklistEntregaDificuldade, tabela linxmicrovix.LinxPedidosVendaChecklistEntregaDificuldade. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_checklist_entrega_dificuldade (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxPedidosVendaChecklistEntregaDificuldade
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public string id_checklist_entrega_dificuldade { get; private set; } = String.Empty;
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxPedidosVendaChecklistEntregaDificuldade() { }

        public LinxPedidosVendaChecklistEntregaDificuldade(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaDificuldade dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_checklist_entrega_dificuldade = MicrovixConverters.ToNullableString(dto.id_checklist_entrega_dificuldade, 1000) ?? String.Empty;   //fora da documentação
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
