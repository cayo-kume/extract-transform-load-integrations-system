namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Consignments
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxRemessasIdentificadores: recebe o identificador da venda e retorna a relação com os identificadores das notas dos itens da remessa. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxRemessasIdentificadores
    {
        public string? identificador_venda { get; init; }
        public string? identificador_remessa { get; init; }
        public string? id_remessas { get; init; }
        public string? id_remessas_acertos { get; init; }
        public string? transacao_acerto { get; init; }
        public string? qtde_total_acerto { get; init; }
        public string? transacao_remessa { get; init; }
        public string? identificador_devolucao { get; init; }
        public string? id_remessa_operacoes { get; init; }
        public string? portal { get; init; }
    }
}
