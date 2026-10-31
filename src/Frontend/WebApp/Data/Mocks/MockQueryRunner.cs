using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WebApp.Models;

namespace WebApp.Data.Mocks
{
    /// <summary>
    /// Executor falso da tela Executar Querys: nada chega a um banco. O SELECT devolve colunas tiradas do próprio SQL e
    /// linhas geradas; as alterações devolvem linhas afetadas e abrem uma transação de mentira, para a tela exercitar
    /// "Confirmar" e "Desfazer". SQL com a palavra "erro" simula um erro do SQL Server.
    /// </summary>
    public partial class MockQueryRunner : IQueryRunner
    {
        private static readonly IReadOnlyList<string> Databases = ["AUDITING", "Company", "NEWBLOOMERS"];

        private static readonly string[] DefaultColumns = ["id", "doc_company", "nome", "valor", "data_atualizacao"];

        private readonly ConcurrentDictionary<Guid, DateTime> _openTransactions = new();

        public async Task<IReadOnlyList<string>> GetDatabasesAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(Random.Shared.Next(100, 250), cancellationToken);
            return Databases;
        }

        public async Task<QueryResult> ExecuteAsync(string database, string sql, CancellationToken cancellationToken = default)
        {
            var watch = Stopwatch.StartNew();
            var seed = (ulong)(uint)$"{database}|{sql}".GetHashCode() ^ (ulong)DateTime.Now.Minute;

            // o mesmo atraso que o banco terá; consultas longas dão tempo de testar o cancelamento
            await Task.Delay(Random.Shared.Next(300, 900) + (sql.Contains("WAITFOR", StringComparison.OrdinalIgnoreCase) ? 8000 : 0), cancellationToken);

            if (sql.Contains("erro", StringComparison.OrdinalIgnoreCase))
            {
                return new QueryResult
                {
                    Kind = SqlInspector.Kind(sql),
                    Error = "Msg 208, Level 16, State 1, Line 1" + Environment.NewLine + "Invalid object name 'dbo.erro'.",
                    Duration = watch.Elapsed
                };
            }

            var kind = SqlInspector.Kind(sql);
            var result = new QueryResult { Kind = kind };

            switch (kind)
            {
                case QueryKind.Select:
                    var columns = Columns(sql);
                    var total = Top(sql) ?? (int)(MockTimeline.Hash(seed) % 2500);
                    result.Columns = columns;
                    result.TotalRows = total;
                    result.Rows = Enumerable.Range(0, Math.Min(total, IQueryRunner.MaxRows))
                        .Select(i => columns.Select(c => Value(c, seed, i)).ToArray())
                        .ToList();
                    result.Messages = [$"({total} linha(s) afetada(s))"];
                    break;

                case QueryKind.Write:
                    result.RowsAffected = (int)(MockTimeline.Hash(seed) % 500);
                    result.Messages = [$"({result.RowsAffected} linha(s) afetada(s))"];
                    break;

                case QueryKind.Ddl:
                    result.Messages = ["Comando executado com êxito."];
                    break;

                case QueryKind.Execute:
                    result.RowsAffected = (int)(MockTimeline.Hash(seed) % 200);
                    result.Messages = ["Procedure iniciada.", $"({result.RowsAffected} linha(s) afetada(s))", "Procedure concluída."];
                    break;
            }

            if (kind != QueryKind.Select)
            {
                // uma procedure com COMMIT próprio fecha a transação da tela, e não há o que desfazer
                if (kind == QueryKind.Execute && sql.Contains("COMMIT", StringComparison.OrdinalIgnoreCase))
                {
                    result.TransactionClosedByCommand = true;
                }
                else
                {
                    var transactionId = Guid.NewGuid();
                    _openTransactions[transactionId] = DateTime.Now;
                    result.TransactionId = transactionId;
                }
            }

            result.Duration = watch.Elapsed;
            return result;
        }

        public Task CommitAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            _openTransactions.TryRemove(transactionId, out _);
            return Task.Delay(150, cancellationToken);
        }

        public Task RollbackAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            _openTransactions.TryRemove(transactionId, out _);
            return Task.Delay(150, cancellationToken);
        }

        // colunas da lista do SELECT (o alias, ou o último nome de cada item); com *, colunas padrão
        private static IReadOnlyList<string> Columns(string sql)
        {
            var match = SelectList().Match(SqlInspector.Normalize(sql));
            if (!match.Success)
                return DefaultColumns;

            var items = match.Groups["list"].Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 0 || items.Any(x => x.EndsWith('*')))
                return DefaultColumns;

            return items
                .Select(x => Alias().Match(x) is { Success: true } alias ? alias.Groups["name"].Value : x.Split('.', ' ').Last())
                .Select(x => x.Trim('[', ']'))
                .ToList();
        }

        private static int? Top(string sql) =>
            TopClause().Match(sql) is { Success: true } top ? int.Parse(top.Groups["n"].Value) : null;

        private static object? Value(string column, ulong seed, int row)
        {
            var h = MockTimeline.Hash(seed ^ ((ulong)row << 20) ^ (ulong)(uint)column.GetHashCode());
            var name = column.ToLowerInvariant();

            if (h % 40 == 0 && !name.StartsWith("id"))
                return null;

            if (name.StartsWith("id") || name.EndsWith("id"))
                return row + 1;

            if (name.Contains("data") || name.Contains("date") || name.Contains("timestamp"))
                return new DateTime(2026, 10, 1).AddMinutes((double)(h % (30 * 24 * 60)));

            if (name.Contains("valor") || name.Contains("preco") || name.Contains("price") || name.Contains("total"))
                return Math.Round((h % 1_000_000) / 100m, 2);

            if (name.Contains("doc") || name.Contains("cnpj"))
                return (10_000_000_000_000 + (long)(h % 89_999_999_999_999)).ToString();

            if (name.Contains("ativo") || name.StartsWith("is"))
                return h % 2 == 0;

            return $"{column}_{h % 10_000}";
        }

        [GeneratedRegex(@"\bSELECT\s+(?:DISTINCT\s+)?(?:TOP\s*\(?\s*\d+\s*\)?\s+)?(?<list>.+?)\s+FROM\b", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
        private static partial Regex SelectList();

        [GeneratedRegex(@"\bAS\s+(?<name>\[?\w+\]?)\s*$", RegexOptions.IgnoreCase)]
        private static partial Regex Alias();

        [GeneratedRegex(@"\bTOP\s*\(?\s*(?<n>\d+)", RegexOptions.IgnoreCase)]
        private static partial Regex TopClause();
    }
}
