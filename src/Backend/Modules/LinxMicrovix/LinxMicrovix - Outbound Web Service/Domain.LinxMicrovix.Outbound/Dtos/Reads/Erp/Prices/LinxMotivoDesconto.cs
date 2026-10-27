namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxMotivoDesconto: os motivos de desconto cadastrados no portal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxMotivoDesconto
    {
        public string? id_motivo_desconto { get; init; }
        public string? desc_motivo_desconto { get; init; }
        public string? ativo { get; init; }
        public string? excluido { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
