namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPlanosBandeiras: o cadastro de bandeiras de cartões vinculadas aos Planos de Pagamento do Portal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPlanosBandeiras
    {
        public string? portal { get; init; }
        public string? plano { get; init; }
        public string? bandeira { get; init; }
        public string? tipo_bandeira { get; init; }
        public string? adquirente { get; init; }
        public string? nome_adquirente { get; init; }
        public string? codigo_bandeira_sitef { get; init; }
        public string? timestamp { get; init; }
    }
}
