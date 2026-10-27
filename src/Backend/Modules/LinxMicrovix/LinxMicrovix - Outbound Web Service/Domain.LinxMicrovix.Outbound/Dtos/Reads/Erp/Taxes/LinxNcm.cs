namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxNcm: o conteúdo da tabela classificacao_ncm_produto. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxNcm
    {
        public string? portal { get; init; }
        public string? codigo { get; init; }
        public string? descricao { get; init; }
        public string? codigo_genero { get; init; }
        public string? ativo { get; init; }
        public string? id_ncm { get; init; }
        public string? timestamp { get; init; }
    }
}
