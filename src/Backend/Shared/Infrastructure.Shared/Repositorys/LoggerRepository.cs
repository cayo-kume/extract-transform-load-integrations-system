using Dapper;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Shared.Entities.Auditing;
using Infrastructure.Shared.Abstractions;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.Shared.Repositorys
{
    public class LoggerRepository : ILoggerRepository
    {
        private const int CommandTimeoutSeconds = 360;

        // a transação inteira repete com estas esperas (.docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md, 3.2)
        private static readonly TimeSpan[] RetryDelays = [TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3)];

        // 1205: deadlock (C2); 4891: "Insert bulk failed due to a schema change of the target table" (C3, Azure SQL);
        // os demais: banco indisponível ou ocupado no Azure SQL (serverless acordando, failover, limite de recursos)
        private static readonly HashSet<int> TransientErrors = [1205, 4891, 40613, 40501, 40197, 49918, 49919, 49920, 10928, 10929];

        private const string InsertLog = """
            INSERT INTO [auditing].[Logs] ([IdJob], [IdType], [Holder], [Company], [StartDate], [EndDate], [Execution])
            VALUES (@IdJob, @IdType, @Holder, @Company, @StartDate, @EndDate, @Execution)
            """;

        private const string UpdateEndDate = """
            UPDATE [auditing].[Logs] SET [EndDate] = @EndDate
            WHERE [IdJob] = @IdJob AND [IdType] = @IdType AND [Holder] = @Holder AND [Company] = @Company AND [Execution] = @Execution
            """;

        private readonly ISQLServerConnection _sqlServerConnection;
        private readonly ISharedRepository _coreRepository;
        private readonly ILogger<LoggerRepository> _logger;

        public LoggerRepository(ISQLServerConnection sqlServerConnection, ISharedRepository coreRepository, ILogger<LoggerRepository> logger) =>
            (_sqlServerConnection, _coreRepository, _logger) = (sqlServerConnection, coreRepository, logger);

        public void LogStart(Log log)
        {
            using var conn = _sqlServerConnection.GetDbAuditingConnection();
            conn.Execute(InsertLog, Parameters(log), commandTimeout: CommandTimeoutSeconds);
        }

        public async Task LogCommit(Log log, IReadOnlyList<Message> messages, IReadOnlyList<Record> records, bool started)
        {
            var recordsTable = _coreRepository.CreateSystemDataTable("Records", new Record());
            var messagesTable = _coreRepository.CreateSystemDataTable("Messages", new Message());

            _coreRepository.FillSystemDataTable(recordsTable, records.ToList());
            _coreRepository.FillSystemDataTable(messagesTable, messages.Where(y => y.Msg != null).ToList());

            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    using var conn = _sqlServerConnection.GetDbAuditingConnection();
                    using var transaction = conn.BeginTransaction();

                    if (!started)
                        await conn.ExecuteAsync(InsertLog, Parameters(log), transaction, CommandTimeoutSeconds);
                    else if (log.EndDate is not null)
                        await conn.ExecuteAsync(UpdateEndDate, Parameters(log), transaction, CommandTimeoutSeconds);

                    BulkInsert(conn, transaction, recordsTable);
                    BulkInsert(conn, transaction, messagesTable);

                    transaction.Commit();
                    return;
                }
                catch (Exception ex) when (attempt < RetryDelays.Length && IsTransient(ex))
                {
                    _logger.LogWarning(ex, "Gravação do log da execução {Execution} (job {IdJob}, empresa {Company}) falhou com um erro transitório; nova tentativa em {Delay} s.",
                        log.Execution, Convert.ToInt32(log.Job), log.Company, RetryDelays[attempt].TotalSeconds);

                    await Task.Delay(RetryDelays[attempt]);
                }
            }
        }

        private static object Parameters(Log log) => new
        {
            IdJob = Convert.ToInt32(log.Job),
            IdType = Convert.ToInt32(log.IdType),
            Holder = log.Holder,
            Company = log.Company,
            StartDate = log.StartDate.ToString("s"),
            EndDate = log.EndDate?.ToString("s"),
            Execution = log.Execution
        };

        private static void BulkInsert(SqlConnection conn, SqlTransaction transaction, DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
                return;

            using var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction)
            {
                DestinationTableName = $"[auditing].[{dataTable.TableName}]",
                BatchSize = dataTable.Rows.Count,
                BulkCopyTimeout = CommandTimeoutSeconds
            };

            foreach (DataColumn c in dataTable.Columns)
                bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);

            bulkCopy.WriteToServer(dataTable);
        }

        private static bool IsTransient(Exception ex) =>
            ex switch
            {
                SqlException sql => sql.Errors.Cast<SqlError>().Any(e => TransientErrors.Contains(e.Number)),
                InvalidOperationException { InnerException: SqlException inner } => IsTransient(inner),
                _ => ex.InnerException is not null && IsTransient(ex.InnerException)
            };
    }
}
