namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxOticoPrismaDescricao: descrição de prismas óticos. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxOticoPrismaDescricao
    {
        public string? id_otico_prisma_descricao { get; init; }
        public string? descricao { get; init; }
        public string? timestamp { get; init; }
        public string? portal { get; init; }
    }
}
