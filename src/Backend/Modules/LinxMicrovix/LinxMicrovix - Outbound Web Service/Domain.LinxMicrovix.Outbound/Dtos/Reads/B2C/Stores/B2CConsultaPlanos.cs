namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaPlanos (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaPlanos
    {
        public string? plano { get; init; }
        public string? nome_plano { get; init; }
        public string? forma_pagamento { get; init; }
        public string? qtde_parcelas { get; init; }
        public string? valor_minimo_parcela { get; init; }
        public string? indice { get; init; }
        public string? timestamp { get; init; }
        public string? desativado { get; init; }
        public string? tipo_plano { get; init; }
        public string? portal { get; init; }
        public string? usa_tef { get; init; }
    }
}
