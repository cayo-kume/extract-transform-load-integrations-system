namespace Worker.Scheduling
{
    /// <summary>
    /// seção Quartz do appsettings.json (src/Backend/Hosts/Worker/docs/PLANO-HOST-WORKER.md §3.1), nossa e estruturada como a do
    /// Hangfire do WebApi: não é a seção de chaves planas (quartz.scheduler.*) da documentação do Quartz.NET. Com Enabled = true ela é
    /// validada na subida (QuartzSettingsValidator)
    /// </summary>
    public class QuartzSettings
    {
        public const string SectionName = "Quartz";

        /// <summary>
        /// false: o host sobe sem o Quartz, sem agendar nada (para conferir configuração e conexão)
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// nome do scheduler, o mesmo em todos os Workers do cluster (cada um tem o seu instanceId, AUTO)
        /// </summary>
        public string SchedulerName { get; set; } = "Worker";

        public string TimeZone { get; set; } = "America/Sao_Paulo";
        public StorageSettings Storage { get; set; } = new();
        public ThreadsSettings Threads { get; set; } = new();
        public MisfireSettings Misfire { get; set; } = new();
        public ScheduleSettings Schedule { get; set; } = new();
        public ConcurrencySettings Concurrency { get; set; } = new();
        public ShutdownSettings Shutdown { get; set; } = new();

        public class StorageSettings
        {
            public string ConnectionStringName { get; set; } = "Connection";

            /// <summary>
            /// schema e prefixo das tabelas do Script001_Create_Quartz_Tables.sql; o Quartz concatena o nome da tabela depois dele
            /// </summary>
            public string TablePrefix { get; set; } = "quartz.QRTZ_";

            /// <summary>
            /// ligado desde o início (§3.3): dois schedulers sem cluster nas mesmas tabelas é configuração que o Quartz não suporta
            /// </summary>
            public bool Clustered { get; set; } = true;

            public int ClusterCheckinIntervalInSeconds { get; set; } = 20;
        }

        public class ThreadsSettings
        {
            public int MaxConcurrency { get; set; } = 20;
        }

        public class MisfireSettings
        {
            /// <summary>
            /// quanto um disparo pode esperar por thread antes de ser descartado (§5.2)
            /// </summary>
            public int ThresholdInMinutes { get; set; } = 15;
        }

        public class ScheduleSettings
        {
            public int RefreshIntervalInMinutes { get; set; } = 10;
        }

        public class ConcurrencySettings
        {
            public int MaxConsecutiveSkips { get; set; } = 5;
        }

        public class ShutdownSettings
        {
            /// <summary>
            /// quanto a parada do host espera as execuções em andamento terminarem
            /// </summary>
            public int TimeoutInMinutes { get; set; } = 5;
        }
    }
}
