namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxServicoAgendaComandas: a agenda das comandas dos serviços. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxServicoAgendaComandas
    {
        public string? portal { get; init; }
        public string? id_comanda { get; init; }
        public string? empresa { get; init; }
        public string? cliente { get; init; }
        public string? data { get; init; }
        public string? noiva { get; init; }
        public string? excluida { get; init; }
        public string? fechada { get; init; }
        public string? observacoes { get; init; }
        public string? tipo_desconto { get; init; }
        public string? valor_desconto { get; init; }
        public string? timestamp { get; init; }
    }
}
