namespace WebApp.Data
{
    /// <summary>
    /// Querys prontas da barra lateral da tela Executar Querys. Clicar numa cola o SQL no editor e seleciona o banco;
    /// executar continua sendo um ato do usuário. A lista é fixa no código: editar o SQL colado não muda o botão.
    /// </summary>
    public static class ReadyQueries
    {
        public const string DefaultDatabase = "NEWBLOOMERS";

        public static readonly IReadOnlyList<ReadyQuery> All =
        [
            new("Auditoria", "Últimas 100 execuções", DefaultDatabase, """
                SELECT TOP 100 J.Title, L.StartDate, L.EndDate
                FROM auditing.Logs L WITH (NOLOCK)
                JOIN auditing.Jobs J WITH (NOLOCK) ON J.IdJob = L.IdJob AND J.IdType = L.IdType
                ORDER BY L.StartDate DESC
                """),
            new("Auditoria", "Erros de hoje", DefaultDatabase, """
                SELECT TOP 100 J.Title, L.StartDate, M.[Message]
                FROM auditing.Messages M WITH (NOLOCK)
                JOIN auditing.Logs L WITH (NOLOCK) ON L.IdJob = M.IdJob AND L.IdType = M.IdType AND L.Execution = M.Execution
                JOIN auditing.Jobs J WITH (NOLOCK) ON J.IdJob = M.IdJob AND J.IdType = M.IdType
                WHERE M.IsError = 1 AND L.StartDate >= CAST(GETDATE() AS DATE)
                ORDER BY L.StartDate DESC
                """),
            new("Auditoria", "Execuções por aplicação (24h)", DefaultDatabase, """
                SELECT A.Title, COUNT(*) AS Execucoes
                FROM auditing.Logs L WITH (NOLOCK)
                JOIN auditing.Jobs J WITH (NOLOCK) ON J.IdJob = L.IdJob AND J.IdType = L.IdType
                JOIN auditing.Apps A WITH (NOLOCK) ON A.IdParent = J.IdParent
                WHERE L.StartDate >= DATEADD(HOUR, -24, GETDATE())
                GROUP BY A.Title
                ORDER BY Execucoes DESC
                """),
            new("Auditoria", "Registros de uma execução", DefaultDatabase, """
                SELECT TOP 100 R.FieldKeyValue, R.RecordText
                FROM auditing.Records R WITH (NOLOCK)
                WHERE R.Execution = '<execution>'
                """),
            new("Jobs", "Jobs por aplicação", DefaultDatabase, """
                SELECT A.Title AS App, J.IdJob, J.Title, J.IntervalTimeInMinutes, T.JobType
                FROM auditing.Jobs J WITH (NOLOCK)
                JOIN auditing.Types T WITH (NOLOCK) ON T.IdType = J.IdType
                LEFT JOIN auditing.Apps A WITH (NOLOCK) ON A.IdParent = J.IdParent
                ORDER BY A.Title, J.IdJob
                """),
            new("Jobs", "Última execução de cada job", DefaultDatabase, """
                SELECT J.IdJob, J.Title, MAX(L.StartDate) AS UltimaExecucao
                FROM auditing.Jobs J WITH (NOLOCK)
                LEFT JOIN auditing.Logs L WITH (NOLOCK) ON L.IdJob = J.IdJob AND L.IdType = J.IdType
                GROUP BY J.IdJob, J.Title
                ORDER BY UltimaExecucao
                """),
            new("Jobs", "Execuções rodando agora", DefaultDatabase, """
                SELECT J.Title, L.StartDate, L.Execution
                FROM auditing.Logs L WITH (NOLOCK)
                JOIN auditing.Jobs J WITH (NOLOCK) ON J.IdJob = L.IdJob AND J.IdType = L.IdType
                WHERE L.EndDate IS NULL
                ORDER BY L.StartDate
                """)
        ];

        public static IEnumerable<IGrouping<string, ReadyQuery>> ByCategory => All.GroupBy(x => x.Category);
    }

    public sealed record ReadyQuery(string Category, string Title, string Database, string Sql);
}
