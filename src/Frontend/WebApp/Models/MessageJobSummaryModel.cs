namespace WebApp.Models
{
    /// <summary>
    /// Totais de mensagens de um job no período, para o cabeçalho de cada job na tela Logs de Erros, com a situação atual
    /// do job (ainda falhando ou recuperado).
    /// </summary>
    public class MessageJobSummaryModel
    {
        public int IdApp { get; set; }
        public int IdJob { get; set; }
        public int IdType { get; set; }
        public string? JobTitle { get; set; }

        public int Errors { get; set; }

        /// <summary>
        /// Zero quando o nível mínimo é de erro.
        /// </summary>
        public int Warnings { get; set; }

        /// <summary>
        /// Causas distintas das mensagens (ver MessageGroupModel).
        /// </summary>
        public int Causes { get; set; }

        /// <summary>
        /// Início da execução mais recente com mensagem do nível pedido, e o texto dela.
        /// </summary>
        public DateTime? LastErrorAt { get; set; }

        public string? LastMessage { get; set; }

        // situação atual do job, a mesma da tela de Jobs
        public bool IsActive { get; set; }
        public JobHealth? CurrentHealth { get; set; }
        public int IntervalTimeInMinutes { get; set; }
        public TimeOnly? RunningDailyAt { get; set; }
        public DateTime? LastStartDate { get; set; }
        public DateTime? LastEndDate { get; set; }
        public AcknowledgementModel? Acknowledgement { get; set; }

        /// <summary>
        /// Integração sob demanda (ver JobModel.IsOnDemand): o cabeçalho ganha o chip e o link vai para a tela Sob Demanda.
        /// </summary>
        public bool IsOnDemand => IdType == 2 && IntervalTimeInMinutes == 0 && RunningDailyAt is null;
    }
}
