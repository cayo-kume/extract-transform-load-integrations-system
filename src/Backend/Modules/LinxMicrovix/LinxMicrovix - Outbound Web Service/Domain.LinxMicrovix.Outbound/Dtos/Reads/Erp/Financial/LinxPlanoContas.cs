namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPlanoContas: as informações do plano de contas cadastrado no portal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPlanoContas
    {
        public string? portal { get; init; }
        public string? empresa { get; init; }
        public string? CNPJ { get; init; }
        public string? id_conta { get; init; }
        public string? nome_conta { get; init; }
        public string? sintetica { get; init; }
        public string? indice { get; init; }
        public string? ativa { get; init; }
        public string? fluxo_caixa { get; init; }
        public string? CONTA_CONTABIL { get; init; }
        public string? id_natureza_conta { get; init; }
        public string? conta_bancaria { get; init; }
        public string? timestamp { get; init; }
    }
}
