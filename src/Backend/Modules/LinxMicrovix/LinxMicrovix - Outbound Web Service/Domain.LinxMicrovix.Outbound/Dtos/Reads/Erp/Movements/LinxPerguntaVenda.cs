namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxPerguntaVenda: as perguntas realizadas no final da venda cadastradas no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxPerguntaVenda
    {
        public string? portal { get; init; }
        public string? id_pergunta_venda { get; init; }
        public string? descricao_pergunta { get; init; }
        public string? timestamp { get; init; }
    }
}
