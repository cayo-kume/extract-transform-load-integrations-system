using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.PosSales
{
    /// <summary>
    /// LinxVendasPosAgrupamentoTmp, tabela linxmicrovix.LinxVendasPosAgrupamentoTmp. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_vendas_pos_agrupamento_tmp (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxVendasPosAgrupamentoTmp
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_vendas_pos_agrupamento_tmp { get; private set; }
        public long? id_vendas_pos { get; private set; }
        public long? id_vendas_pos_origem { get; private set; }
        public long timestamp { get; private set; }

        public LinxVendasPosAgrupamentoTmp() { }

        public LinxVendasPosAgrupamentoTmp(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.PosSales.LinxVendasPosAgrupamentoTmp dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_vendas_pos_agrupamento_tmp = MicrovixConverters.ToInt32(dto.id_vendas_pos_agrupamento_tmp);
            id_vendas_pos = MicrovixConverters.ToNullableInt64(dto.id_vendas_pos);
            id_vendas_pos_origem = MicrovixConverters.ToNullableInt64(dto.id_vendas_pos_origem);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
