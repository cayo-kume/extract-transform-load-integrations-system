namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxSpedTipoItem: dados do Sped tipo item. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxSpedTipoItem
    {
        public string? portal { get; init; }
        public string? id_sped_tipo_item { get; init; }
        public string? codigo { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
    }
}
