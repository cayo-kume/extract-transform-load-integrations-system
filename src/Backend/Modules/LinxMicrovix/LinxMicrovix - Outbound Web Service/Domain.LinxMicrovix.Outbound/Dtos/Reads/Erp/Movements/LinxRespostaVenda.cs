namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxRespostaVenda: o cadastro das perguntas realizadas no final da venda no portal. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxRespostaVenda
    {
        public string? portal { get; init; }
        public string? id_resposta_venda { get; init; }
        public string? descricao_resposta { get; init; }
        public string? id_pergunta_venda { get; init; }
        public string? timestamp { get; init; }
    }
}
