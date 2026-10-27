using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// LinxValeOrdemServicoExterna, tabela linxmicrovix.LinxValeOrdemServicoExterna. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_vale_ordem_servico_externa (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxValeOrdemServicoExterna
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_vale_ordem_servico_externa { get; private set; }
        public string? numero_controle { get; private set; }
        public long timestamp { get; private set; }

        public LinxValeOrdemServicoExterna() { }

        public LinxValeOrdemServicoExterna(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ServiceOrders.LinxValeOrdemServicoExterna dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_vale_ordem_servico_externa = MicrovixConverters.ToInt32(dto.id_vale_ordem_servico_externa);
            numero_controle = MicrovixConverters.ToNullableString(dto.numero_controle, 25);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
