namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxSpedTipoBaseCredito: o conteúdo da tabela sped_tipo_base_credito. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxSpedTipoBaseCredito
    {
        public string? portal { get; init; }
        public string? id_sped_tipo_base_credito { get; init; }
        public string? codigo_sped_tipo_base_credito { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
    }
}
