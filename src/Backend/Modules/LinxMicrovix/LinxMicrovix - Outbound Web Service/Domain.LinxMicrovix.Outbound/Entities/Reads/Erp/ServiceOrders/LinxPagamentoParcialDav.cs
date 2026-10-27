using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// LinxPagamentoParcialDav, tabela linxmicrovix.LinxPagamentoParcialDav. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_pagamento_parcial_tmp
    /// </summary>
    public class LinxPagamentoParcialDav
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int? id_vendas_pos { get; private set; }
        public decimal? valor { get; private set; }
        public decimal? ajuste_valor_desc_acresc_plano { get; private set; }
        public long timestamp { get; private set; }
        public int id_pagamento_parcial_tmp { get; private set; }
        public int? plano { get; private set; }
        public string? forma_pgto { get; private set; }
        public decimal? valor_2 { get; private set; }
        public int? id_bandeira { get; private set; }
        public int? qtde_parcelas { get; private set; }
        public string? credito_debito { get; private set; }
        public int portal { get; private set; }

        public LinxPagamentoParcialDav() { }

        public LinxPagamentoParcialDav(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ServiceOrders.LinxPagamentoParcialDav dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_vendas_pos = MicrovixConverters.ToNullableInt32(dto.id_vendas_pos);
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            ajuste_valor_desc_acresc_plano = MicrovixConverters.ToNullableDecimal(dto.ajuste_valor_desc_acresc_plano);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            id_pagamento_parcial_tmp = MicrovixConverters.ToInt32(dto.id_pagamento_parcial_tmp);
            plano = MicrovixConverters.ToNullableInt32(dto.plano);
            forma_pgto = MicrovixConverters.ToNullableString(dto.forma_pgto, 50);
            valor_2 = MicrovixConverters.ToNullableDecimal(dto.valor_2);   //fora da documentação
            id_bandeira = MicrovixConverters.ToNullableInt32(dto.id_bandeira);
            qtde_parcelas = MicrovixConverters.ToNullableInt32(dto.qtde_parcelas);
            credito_debito = MicrovixConverters.ToNullableString(dto.credito_debito, 1);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
