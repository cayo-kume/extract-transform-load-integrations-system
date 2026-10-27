namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxECF: as ECFs cadastradas para a empresa. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxECF
    {
        public string? id_ecf { get; init; }
        public string? empresa { get; init; }
        public string? ecf { get; init; }
        public string? numeroserie { get; init; }
        public string? indice_sangria { get; init; }
        public string? modelo { get; init; }
        public string? nome { get; init; }
        public string? indice_suprimento { get; init; }
        public string? ativo { get; init; }
        public string? indice_sinal { get; init; }
        public string? indice_antecipacao { get; init; }
        public string? indice_cartao_credito { get; init; }
        public string? empresa_ecf { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
