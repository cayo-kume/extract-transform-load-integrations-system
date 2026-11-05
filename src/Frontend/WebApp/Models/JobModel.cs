namespace WebApp.Models
{
    /// <summary>
    /// Job de auditing.Jobs, com a última execução e a saúde calculada a partir dela.
    /// </summary>
    public class JobModel
    {
        /// <summary>
        /// Coluna IdParent: nas integrações é a aplicação (auditing.Apps); nas procedures é o job de integração que as chama.
        /// </summary>
        public int IdParent { get; set; }

        /// <summary>
        /// Aplicação do job, já resolvida: o IdParent nas integrações e a aplicação do job pai nas procedures.
        /// </summary>
        public int IdApp { get; set; }

        /// <summary>
        /// CNPJ da empresa da linha (auditing.Jobs.Company). Na visão "Todas as empresas" da holding, diz de qual empresa é a
        /// linha e para qual empresa vai cada ação dela.
        /// </summary>
        public string Company { get; set; } = "";

        public int IdJob { get; set; }
        public int IdType { get; set; }
        public string? JobType { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// 0 = sob demanda.
        /// </summary>
        public int IntervalTimeInMinutes { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Preenchida, o job é diário e roda nessa hora.
        /// </summary>
        public TimeOnly? RunningDailyAt { get; set; }

        public string? Queue { get; set; }

        public ExecutionModel? LastExecution { get; set; }

        /// <summary>
        /// Nula para job inativo ou sob demanda que ainda não rodou.
        /// </summary>
        public JobHealth? Health { get; set; }

        /// <summary>
        /// Integração sem agenda (intervalo 0, sem hora diária): só roda quando alguém chama, pela tela Sob Demanda. Fica fora
        /// das grids de Jobs, Integrações, Agenda e do painel de saúde.
        /// </summary>
        public bool IsOnDemand => IdType == 2 && IntervalTimeInMinutes == 0 && RunningDailyAt is null;

        /// <summary>
        /// Falha reconhecida por quem opera o painel; nulo quando não há reconhecimento válido.
        /// </summary>
        public AcknowledgementModel? Acknowledgement { get; set; }

        /// <summary>
        /// Pedido de "Executar agora" ainda não atendido.
        /// </summary>
        public RunRequestModel? RunRequest { get; set; }
    }
}
