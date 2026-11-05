namespace WebApp.Data
{
    /// <summary>
    /// Querys prontas da barra lateral da tela Executar Querys. Clicar numa cola o SQL no editor e seleciona o banco do painel;
    /// executar continua sendo um ato do usuário. A lista é fixa no código: editar o SQL colado não muda o botão.
    /// </summary>
    public static class ReadyQueries
    {
        /// <summary>
        /// Query "Registros de uma execução", aberta pelos Logs de Erros em /queries?ready=...&amp;execution=...
        /// </summary>
        public const string ExecutionRecordsKey = "registros-execucao";

        /// <summary>
        /// Marcador da execução no SQL das querys prontas.
        /// </summary>
        public const string ExecutionPlaceholder = "<execution>";

        // todas no banco do painel (IQueryRunner.DefaultDatabase). Título e IdParent dos jobs ficam em auditing.Catalog
        // (por IdJob + IdType); auditing.Jobs, Logs, Messages e Records levam Holder e Company na chave
        public static readonly IReadOnlyList<ReadyQuery> All =
        [
            new("Auditoria", "Últimas 100 execuções", """
                SELECT TOP 100 E.FantasyName AS Empresa, C.Title, L.StartDate, L.EndDate
                FROM auditing.Logs L WITH (NOLOCK)
                JOIN auditing.Catalog C WITH (NOLOCK) ON C.IdJob = L.IdJob AND C.IdType = L.IdType
                LEFT JOIN auditing.Companies E WITH (NOLOCK) ON E.Company = L.Company
                ORDER BY L.StartDate DESC
                """),
            new("Auditoria", "Erros de hoje", """
                SELECT TOP 100 E.FantasyName AS Empresa, C.Title, L.StartDate, M.[Message]
                FROM auditing.Messages M WITH (NOLOCK)
                JOIN auditing.Logs L WITH (NOLOCK) ON L.IdJob = M.IdJob AND L.IdType = M.IdType AND L.Holder = M.Holder AND L.Company = M.Company AND L.Execution = M.Execution
                JOIN auditing.Catalog C WITH (NOLOCK) ON C.IdJob = M.IdJob AND C.IdType = M.IdType
                LEFT JOIN auditing.Companies E WITH (NOLOCK) ON E.Company = M.Company
                WHERE M.[Level] = 3 AND L.StartDate >= CAST(CAST(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time' AS DATETIME) AS DATE)
                ORDER BY L.StartDate DESC
                """),
            new("Auditoria", "Execuções por aplicação (24h)", """
                SELECT E.FantasyName AS Empresa, A.Title, COUNT(*) AS Execucoes
                FROM auditing.Logs L WITH (NOLOCK)
                JOIN auditing.Catalog C WITH (NOLOCK) ON C.IdJob = L.IdJob AND C.IdType = L.IdType
                JOIN auditing.Apps A WITH (NOLOCK) ON A.IdParent = C.IdParent
                LEFT JOIN auditing.Companies E WITH (NOLOCK) ON E.Company = L.Company
                WHERE L.StartDate >= DATEADD(HOUR, -24, CAST(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time' AS DATETIME))
                GROUP BY E.FantasyName, A.Title
                ORDER BY Execucoes DESC
                """),
            new("Auditoria", "Registros de uma execução", """
                SELECT TOP 100 R.Company, R.FieldKeyValue, R.RecordText
                FROM auditing.Records R WITH (NOLOCK)
                WHERE R.Execution = '<execution>'
                """, ExecutionRecordsKey),
            new("Jobs", "Jobs por aplicação", """
                SELECT A.Title AS App, E.FantasyName AS Empresa, J.IdJob, C.Title, J.IntervalTimeInMinutes, J.RunningDailyAt, J.IsActive, T.JobType
                FROM auditing.Jobs J WITH (NOLOCK)
                JOIN auditing.Catalog C WITH (NOLOCK) ON C.IdJob = J.IdJob AND C.IdType = J.IdType
                JOIN auditing.Types T WITH (NOLOCK) ON T.IdType = J.IdType
                LEFT JOIN auditing.Apps A WITH (NOLOCK) ON A.IdParent = C.IdParent
                LEFT JOIN auditing.Companies E WITH (NOLOCK) ON E.Company = J.Company
                ORDER BY A.Title, E.FantasyName, J.IdJob
                """),
            new("Jobs", "Última execução de cada job", """
                SELECT E.FantasyName AS Empresa, J.IdJob, C.Title, MAX(L.StartDate) AS UltimaExecucao
                FROM auditing.Jobs J WITH (NOLOCK)
                JOIN auditing.Catalog C WITH (NOLOCK) ON C.IdJob = J.IdJob AND C.IdType = J.IdType
                LEFT JOIN auditing.Logs L WITH (NOLOCK) ON L.IdJob = J.IdJob AND L.IdType = J.IdType AND L.Holder = J.Holder AND L.Company = J.Company
                LEFT JOIN auditing.Companies E WITH (NOLOCK) ON E.Company = J.Company
                GROUP BY E.FantasyName, J.IdJob, J.IdType, C.Title
                ORDER BY UltimaExecucao
                """),
            new("Jobs", "Execuções rodando agora", """
                SELECT E.FantasyName AS Empresa, C.Title, L.StartDate, L.Execution
                FROM auditing.Logs L WITH (NOLOCK)
                JOIN auditing.Catalog C WITH (NOLOCK) ON C.IdJob = L.IdJob AND C.IdType = L.IdType
                LEFT JOIN auditing.Companies E WITH (NOLOCK) ON E.Company = L.Company
                WHERE L.EndDate IS NULL
                ORDER BY L.StartDate
                """)
        ];

        public static IEnumerable<IGrouping<string, ReadyQuery>> ByCategory => All.GroupBy(x => x.Category);

        public static ReadyQuery? Find(string? key) => key is null ? null : All.FirstOrDefault(x => x.Key == key);
    }

    /// <summary>
    /// Key identifica a query num link (?ready=); só as que outras telas abrem precisam de uma.
    /// </summary>
    public sealed record ReadyQuery(string Category, string Title, string Sql, string? Key = null);
}
