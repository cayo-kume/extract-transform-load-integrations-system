namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxDesfazimentosAntecipacoesItens: os valores registrados na tabela desfazimentos_antecipacoes_itens (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxDesfazimentosAntecipacoesItens
    {
        public string? portal { get; init; }
        public string? id_desfazimentos_antecipacoes_itens { get; init; }
        public string? id_desfazimentos_antecipacoes { get; init; }
        public string? numero_sequencial { get; init; }
        public string? ordem { get; init; }
        public string? identificador { get; init; }
        public string? preco { get; init; }
        public string? quantidade { get; init; }
        public string? desconto { get; init; }
        public string? valor { get; init; }
        public string? timestamp { get; init; }
    }
}
