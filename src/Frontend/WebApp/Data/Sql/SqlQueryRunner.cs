using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Data.Sql
{
    /// <summary>
    /// IQueryRunner no SQL Server, com a connection string própria da tela (ConnectionStrings:QueryConnection, usuário
    /// etl_query, docs/PLANO-WEBAPP.md, passo 12). O banco escolhido entra na conexão (InitialCatalog), nunca por USE no
    /// texto. Todo comando roda numa transação: o SELECT é desfeito no fim (se o texto esconder uma escrita, ela não fica), e
    /// o que altera algo fica aberto esperando CommitAsync ou RollbackAsync. Sem resposta, a transação é desfeita aqui
    /// mesmo depois do prazo da tela, para uma aba fechada não deixar tabela travada.
    /// </summary>
    public class SqlQueryRunner(IConfiguration configuration, ILogger<SqlQueryRunner> logger) : IQueryRunner, IDisposable
    {
        // até a fase 6 (autenticação), quem executa é um nome fixo, como no reconhecimento de falhas
        private const string User = "operador";
        private const int CommandTimeoutSeconds = 30;

        // além do prazo da tela, a folga para o "Desfazer" automático da própria tela chegar primeiro
        private static readonly TimeSpan AbandonedAfter = IQueryRunner.ConfirmationTimeout + TimeSpan.FromSeconds(15);

        private readonly string _connectionString = configuration.GetConnectionString("QueryConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:QueryConnection não configurada: a tela Executar Querys com banco precisa dela (usuário etl_query).");

        private readonly ConcurrentDictionary<Guid, OpenTransaction> _open = new();

        public async Task<IReadOnlyList<string>> GetDatabasesAsync(CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(
                "SELECT name FROM sys.databases WHERE database_id > 4 AND state_desc = 'ONLINE' AND HAS_DBACCESS(name) = 1 ORDER BY name",
                connection);

            var databases = new List<string>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
                databases.Add(reader.GetString(0));

            return databases;
        }

        public async Task<QueryResult> ExecuteAsync(string database, string sql, CancellationToken cancellationToken = default)
        {
            var watch = Stopwatch.StartNew();
            var kind = SqlInspector.Kind(sql);
            var result = new QueryResult { Kind = kind };
            var messages = new List<string>();

            // o banco só entra se o etl_query tiver acesso a ele (a mesma lista do seletor)
            if (!(await GetDatabasesAsync(cancellationToken)).Contains(database))
                return new QueryResult { Kind = kind, Error = $"O banco {database} não existe ou não está liberado para a tela.", Duration = watch.Elapsed };

            var connection = new SqlConnection(new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = database }.ConnectionString);
            connection.InfoMessage += (_, e) => messages.AddRange(e.Errors.Cast<SqlError>().Select(x => x.Message));

            SqlTransaction? transaction = null;
            var keepOpen = false;

            try
            {
                await connection.OpenAsync(cancellationToken);
                transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);

                await using (var command = new SqlCommand(sql, connection, transaction) { CommandTimeout = CommandTimeoutSeconds })
                {
                    command.StatementCompleted += (_, e) => messages.Add($"({e.RecordCount} linha(s) afetada(s))");

                    await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                    await ReadFirstResultAsync(reader, result, cancellationToken);

                    // os outros resultados não aparecem, mas precisam ser lidos para os erros e as contagens chegarem
                    while (await reader.NextResultAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken)) { }
                    }

                    result.RowsAffected = reader.RecordsAffected >= 0 && kind != QueryKind.Select ? reader.RecordsAffected : null;
                }

                result.Messages = messages;

                if (kind == QueryKind.Select)
                {
                    await RollbackQuietlyAsync(transaction);
                }
                else if (transaction.Connection is null || await TransactionCountAsync(connection, transaction, cancellationToken) == 0)
                {
                    // uma procedure com COMMIT ou ROLLBACK próprio já encerrou a transação da tela: não há o que desfazer
                    result.TransactionClosedByCommand = true;
                }
                else
                {
                    var id = Guid.NewGuid();
                    var open = new OpenTransaction(connection, transaction, database, sql, kind, result.RowsAffected, MainDatabase.LocalNow());
                    open.Abandon = new Timer(_ => _ = FinishAsync(id, commit: false, abandoned: true), null, AbandonedAfter, Timeout.InfiniteTimeSpan);
                    _open[id] = open;
                    result.TransactionId = id;
                    keepOpen = true;
                }

                result.Duration = watch.Elapsed;
                Log(database, sql, kind, result.Duration, result.RowsAffected, keepOpen ? "aguardando Confirmar ou Desfazer" : result.TransactionClosedByCommand ? "encerrado pelo próprio comando" : "ROLLBACK (consulta)");
                return result;
            }
            catch (SqlException exception)
            {
                if (transaction is not null)
                    await RollbackQuietlyAsync(transaction);

                Log(database, sql, kind, watch.Elapsed, null, $"erro {exception.Number}, ROLLBACK");

                var first = exception.Errors.Count > 0 ? exception.Errors[0] : null;
                return new QueryResult
                {
                    Kind = kind,
                    Error = first is null
                        ? exception.Message
                        : $"Msg {first.Number}, Level {first.Class}, State {first.State}, Line {first.LineNumber}{Environment.NewLine}{exception.Message}",
                    Messages = messages,
                    Duration = watch.Elapsed
                };
            }
            catch (OperationCanceledException)
            {
                if (transaction is not null)
                    await RollbackQuietlyAsync(transaction);

                Log(database, sql, kind, watch.Elapsed, null, "cancelado, ROLLBACK");
                throw;
            }
            finally
            {
                if (!keepOpen)
                {
                    transaction?.Dispose();
                    await connection.DisposeAsync();
                }
            }
        }

        public Task CommitAsync(Guid transactionId, CancellationToken cancellationToken = default) =>
            FinishAsync(transactionId, commit: true, abandoned: false);

        public Task RollbackAsync(Guid transactionId, CancellationToken cancellationToken = default) =>
            FinishAsync(transactionId, commit: false, abandoned: false);

        public void Dispose()
        {
            foreach (var id in _open.Keys)
                FinishAsync(id, commit: false, abandoned: true).GetAwaiter().GetResult();
        }

        // até MaxRows linhas para a tela; o resto só entra na contagem
        private static async Task ReadFirstResultAsync(SqlDataReader reader, QueryResult result, CancellationToken cancellationToken)
        {
            if (reader.FieldCount == 0)
                return;

            result.Columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

            var rows = new List<object?[]>();
            var total = 0;

            while (await reader.ReadAsync(cancellationToken))
            {
                total++;
                if (rows.Count >= IQueryRunner.MaxRows)
                    continue;

                var values = new object?[reader.FieldCount];
                reader.GetValues(values!);
                rows.Add(values.Select(x => x is DBNull ? null : x).ToArray());
            }

            result.Rows = rows;
            result.TotalRows = total;
        }

        private async Task FinishAsync(Guid id, bool commit, bool abandoned)
        {
            if (!_open.TryRemove(id, out var open))
            {
                // a tela pediu depois do prazo: o "Desfazer" automático daqui já encerrou
                if (commit)
                    throw new InvalidOperationException("A transação não está mais aberta: o prazo para confirmar acabou e ela foi desfeita.");
                return;
            }

            await open.Abandon!.DisposeAsync();

            try
            {
                if (commit)
                    await open.Transaction.CommitAsync();
                else
                    await RollbackQuietlyAsync(open.Transaction);

                Log(open.Database, open.Sql, open.Kind, MainDatabase.LocalNow() - open.StartedAt, open.RowsAffected,
                    commit ? "COMMIT" : abandoned ? "ROLLBACK (sem resposta da tela)" : "ROLLBACK");
            }
            finally
            {
                await open.Transaction.DisposeAsync();
                await open.Connection.DisposeAsync();
            }
        }

        private static async Task<int> TransactionCountAsync(SqlConnection connection, SqlTransaction transaction, CancellationToken cancellationToken)
        {
            await using var command = new SqlCommand("SELECT @@TRANCOUNT", connection, transaction);
            return (int)(await command.ExecuteScalarAsync(cancellationToken))!;
        }

        private static async Task RollbackQuietlyAsync(SqlTransaction transaction)
        {
            try
            {
                if (transaction.Connection is not null)
                    await transaction.RollbackAsync();
            }
            catch (Exception exception) when (exception is InvalidOperationException or SqlException)
            {
                // a transação já tinha terminado (erro grave do SQL Server ou COMMIT do próprio comando)
            }
        }

        private void Log(string database, string sql, QueryKind kind, TimeSpan duration, int? rowsAffected, string outcome) =>
            logger.LogInformation("Executar Querys: {User} em {Database}, {Kind}, {Duration} ms, {RowsAffected} linha(s), {Outcome}. SQL: {Sql}",
                User, database, kind, (long)duration.TotalMilliseconds, rowsAffected, outcome, sql);

        private sealed record OpenTransaction(SqlConnection Connection, SqlTransaction Transaction, string Database, string Sql, QueryKind Kind, int? RowsAffected, DateTime StartedAt)
        {
            public Timer? Abandon { get; set; }
        }
    }
}
