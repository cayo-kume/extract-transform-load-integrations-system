using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders
{
    /// <summary>
    /// LinxPedidosVendaChecklistEntregaLocal, tabela linxmicrovix.LinxPedidosVendaChecklistEntregaLocal. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_checklist_entrega_local (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxPedidosVendaChecklistEntregaLocal
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_checklist_entrega_local { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxPedidosVendaChecklistEntregaLocal() { }

        public LinxPedidosVendaChecklistEntregaLocal(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaLocal dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_checklist_entrega_local = MicrovixConverters.ToInt32(dto.id_checklist_entrega_local);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
