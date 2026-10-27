using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Orders
{
    /// <summary>
    /// LinxPlanosPedidoVenda, tabela linxmicrovix.LinxPlanosPedidoVenda. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_pedido + plano (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxPlanosPedidoVenda
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int cod_pedido { get; private set; }
        public int plano { get; private set; }
        public string? desc_plano { get; private set; }
        public decimal? total { get; private set; }
        public int? qtde_parcelas { get; private set; }
        public decimal? indice_plano { get; private set; }
        public decimal? valor_desc_acresc_plano { get; private set; }
        public int? cod_forma_pgto { get; private set; }
        public string? forma_pgto { get; private set; }
        public long timestamp { get; private set; }

        public LinxPlanosPedidoVenda() { }

        public LinxPlanosPedidoVenda(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Orders.LinxPlanosPedidoVenda dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_pedido = MicrovixConverters.ToInt32(dto.cod_pedido);
            plano = MicrovixConverters.ToInt32(dto.plano);
            desc_plano = MicrovixConverters.ToNullableString(dto.desc_plano, 100);
            total = MicrovixConverters.ToNullableDecimal(dto.total);
            qtde_parcelas = MicrovixConverters.ToNullableInt32(dto.qtde_parcelas);
            indice_plano = MicrovixConverters.ToNullableDecimal(dto.indice_plano);
            valor_desc_acresc_plano = MicrovixConverters.ToNullableDecimal(dto.valor_desc_acresc_plano);
            cod_forma_pgto = MicrovixConverters.ToNullableInt32(dto.cod_forma_pgto);
            forma_pgto = MicrovixConverters.ToNullableString(dto.forma_pgto, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
