namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaPedidosPlanos (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaPedidosPlanos
    {
        public string? id_pedido_planos { get; init; }
        public string? id_pedido { get; init; }
        public string? plano_pagamento { get; init; }
        public string? valor_plano { get; init; }
        public string? nsu_sitef { get; init; }
        public string? cod_autorizacao { get; init; }
        public string? texto_comprovante { get; init; }
        public string? cod_loja_sitef { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
