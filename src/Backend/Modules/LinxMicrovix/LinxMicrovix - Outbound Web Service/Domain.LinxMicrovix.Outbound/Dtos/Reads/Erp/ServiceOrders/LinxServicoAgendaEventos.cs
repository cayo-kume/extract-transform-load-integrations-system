namespace Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// uma linha &lt;R&gt; do LinxServicoAgendaEventos: a agenda dos eventos dos serviços. (colunas da resposta real, 27/10/2026)
    /// </summary>
    public class LinxServicoAgendaEventos
    {
        public string? portal { get; init; }
        public string? id_evento { get; init; }
        public string? id_espaco { get; init; }
        public string? data_evento { get; init; }
        public string? minutos_duracao { get; init; }
        public string? tipo_agendamento { get; init; }
        public string? id_cliente_servico { get; init; }
        public string? id_profissional { get; init; }
        public string? situacao { get; init; }
        public string? data_situacao { get; init; }
        public string? observacao { get; init; }
        public string? data_chegada { get; init; }
        public string? data_execucao { get; init; }
        public string? recorrente { get; init; }
        public string? id_comanda { get; init; }
        public string? timestamp { get; init; }
    }
}
