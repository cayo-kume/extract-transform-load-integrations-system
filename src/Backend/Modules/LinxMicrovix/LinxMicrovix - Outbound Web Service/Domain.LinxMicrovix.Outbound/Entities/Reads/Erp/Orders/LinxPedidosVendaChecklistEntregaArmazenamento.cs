using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders
{
    /// <summary>
    /// LinxPedidosVendaChecklistEntregaArmazenamento, tabela linxmicrovix.LinxPedidosVendaChecklistEntregaArmazenamento. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_checklist_entrega_armazenamento (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxPedidosVendaChecklistEntregaArmazenamento
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_checklist_entrega_armazenamento { get; private set; }
        public string? descricao { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxPedidosVendaChecklistEntregaArmazenamento() { }

        public LinxPedidosVendaChecklistEntregaArmazenamento(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Orders.LinxPedidosVendaChecklistEntregaArmazenamento dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_checklist_entrega_armazenamento = MicrovixConverters.ToInt32(dto.id_checklist_entrega_armazenamento);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 100);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
