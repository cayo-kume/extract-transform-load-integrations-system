namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxOrcamentoPosicao: os dados das posições cadastradas para os orçamentos. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxOrcamentoPosicao
    {
        public string? id_posicao { get; init; }
        public string? descricao_posicao { get; init; }
        public string? ativo_posicao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
