namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxTradeinParceiro: os registros cadastrados na tabela tradein_parceiro do portal (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxTradeinParceiro
    {
        public string? portal { get; init; }
        public string? id_tradein_parceiro { get; init; }
        public string? nome_parceiro { get; init; }
        public string? timestamp { get; init; }
    }
}
