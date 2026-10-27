namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores
{
    /// <summary>
    /// uma linha &lt;R&gt; do B2CConsultaPlanosParcelas (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class B2CConsultaPlanosParcelas
    {
        public string? plano { get; init; }
        public string? ordem_parcela { get; init; }
        public string? prazo_parc { get; init; }
        public string? id_planos_parcelas { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
