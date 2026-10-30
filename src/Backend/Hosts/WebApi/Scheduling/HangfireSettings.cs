namespace WebApi.Scheduling
{
    /// <summary>
    /// seção Hangfire do appsettings.json (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §3.1). Com Enabled = false o host sobe
    /// só como API e nada daqui é lido; com true, a seção é validada na subida (HangfireSettingsValidator)
    /// </summary>
    public class HangfireSettings
    {
        public const string SectionName = "Hangfire";

        public bool Enabled { get; set; }
        public string TimeZone { get; set; } = "America/Sao_Paulo";
        public StorageSettings Storage { get; set; } = new();
        public ServerSettings Server { get; set; } = new();
        public ScheduleSettings Schedule { get; set; } = new();
        public ConcurrencySettings Concurrency { get; set; } = new();
        public MaintenanceSettings Maintenance { get; set; } = new();
        public DashboardSettings Dashboard { get; set; } = new();

        public class StorageSettings
        {
            public string ConnectionStringName { get; set; } = "Connection";
            public string SchemaName { get; set; } = "HangFire";
            public bool PrepareSchemaIfNecessary { get; set; } = true;
            public int CommandBatchMaxTimeoutInMinutes { get; set; } = 5;
            public int SlidingInvisibilityTimeoutInMinutes { get; set; } = 5;
            public int JobExpirationTimeoutInMinutes { get; set; } = 30;
            public int QueuePollIntervalInSeconds { get; set; } = 15;
        }

        public class ServerSettings
        {
            public string? ServerName { get; set; }
            // sem valor padrão: o binder soma os itens do appsettings.json aos que já estão no array, e a fila sairia repetida
            public string[] Queues { get; set; } = [];
            public int WorkerCount { get; set; } = 20;
        }

        public class ScheduleSettings
        {
            public int RefreshIntervalInMinutes { get; set; } = 10;
        }

        public class ConcurrencySettings
        {
            public int MaxConsecutiveSkips { get; set; } = 5;
        }

        public class MaintenanceSettings
        {
            public int FailedJobsRetentionInDays { get; set; } = 2;
        }

        public class DashboardSettings
        {
            public string Path { get; set; } = "/hangfire";
            public bool ReadOnly { get; set; }
            public int StatsPollingIntervalInSeconds { get; set; } = 30;

            /// <summary>
            /// credenciais do dashboard (§3.2): vazias no appsettings.json, vêm de variável de ambiente (Hangfire__Dashboard__UserName e
            /// Hangfire__Dashboard__Password) ou de user-secrets
            /// </summary>
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }
    }
}
