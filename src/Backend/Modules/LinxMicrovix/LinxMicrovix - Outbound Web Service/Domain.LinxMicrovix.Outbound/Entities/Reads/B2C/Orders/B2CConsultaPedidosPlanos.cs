using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Orders
{
    /// <summary>
    /// B2CConsultaPedidosPlanos, tabela linxmicrovix.B2CConsultaPedidosPlanos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_pedido_planos
    /// </summary>
    public class B2CConsultaPedidosPlanos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public long id_pedido_planos { get; private set; }
        public int? id_pedido { get; private set; }
        public int? plano_pagamento { get; private set; }
        public decimal? valor_plano { get; private set; }
        public string? nsu_sitef { get; private set; }
        public string? cod_autorizacao { get; private set; }
        public string? texto_comprovante { get; private set; }
        public string? cod_loja_sitef { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaPedidosPlanos() { }

        public B2CConsultaPedidosPlanos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Orders.B2CConsultaPedidosPlanos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_pedido_planos = MicrovixConverters.ToInt64(dto.id_pedido_planos);
            id_pedido = MicrovixConverters.ToNullableInt32(dto.id_pedido);
            plano_pagamento = MicrovixConverters.ToNullableInt32(dto.plano_pagamento);
            valor_plano = MicrovixConverters.ToNullableDecimal(dto.valor_plano);
            nsu_sitef = MicrovixConverters.ToNullableString(dto.nsu_sitef, 20);
            cod_autorizacao = MicrovixConverters.ToNullableString(dto.cod_autorizacao, 50);
            texto_comprovante = String.IsNullOrEmpty(dto.texto_comprovante) ? null : dto.texto_comprovante;
            cod_loja_sitef = MicrovixConverters.ToNullableString(dto.cod_loja_sitef, 10);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
