using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// LinxOrdemServicoExternaDav, tabela linxmicrovix.LinxOrdemServicoExternaDav. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_vendas_pos_tmp_ordem_servico_externa (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxOrdemServicoExternaDav
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_vendas_pos_tmp_ordem_servico_externa { get; private set; }
        public long? id_vendas_pos { get; private set; }
        public string? codigo_ordem_servico_externa { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxOrdemServicoExternaDav() { }

        public LinxOrdemServicoExternaDav(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ServiceOrders.LinxOrdemServicoExternaDav dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_vendas_pos_tmp_ordem_servico_externa = MicrovixConverters.ToInt32(dto.id_vendas_pos_tmp_ordem_servico_externa);
            id_vendas_pos = MicrovixConverters.ToNullableInt64(dto.id_vendas_pos);
            codigo_ordem_servico_externa = MicrovixConverters.ToNullableString(dto.codigo_ordem_servico_externa, 20);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
