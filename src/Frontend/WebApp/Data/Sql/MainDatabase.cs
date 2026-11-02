using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Data.Sql
{
    /// <summary>
    /// Conexão com o banco principal (ConnectionStrings:Connection, usuário etl_monitor), onde ficam auditing.* e monitor.*,
    /// a hora local das tabelas e a tradução dos THROW das procedures do cadastro (50002, 50003, 50004) em
    /// CompanyRegistrationException, com a mensagem da procedure.
    /// </summary>
    public class MainDatabase(IConfiguration configuration)
    {
        // a mesma hora local que os DEFAULT das tabelas de monitor gravam
        public const string Now = "CAST(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time' AS datetime)";

        private static readonly TimeZoneInfo Brasilia = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        private readonly string _connectionString = configuration.GetConnectionString("Connection")
            ?? throw new InvalidOperationException("ConnectionStrings:Connection não configurada: o painel com banco precisa dela.");

        /// <summary>
        /// Agora no horário de Brasília, o mesmo das datas de auditing.Logs, qualquer que seja o fuso do servidor do WebApp.
        /// </summary>
        public static DateTime LocalNow() => TimeZoneInfo.ConvertTime(DateTime.UtcNow, Brasilia);

        public async Task<SqlConnection> OpenAsync(CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }

        public static async Task<T> Translate<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (SqlException exception) when (exception.Number is 50002 or 50003 or 50004)
            {
                throw new CompanyRegistrationException(exception.Message, exception.Number, exception);
            }
        }

        /// <summary>
        /// Tabelas *Parameters de ModuleDefinitions que existem neste banco (um módulo sem migrator rodado não tem a dele).
        /// </summary>
        public static async Task<HashSet<string>> ExistingTablesAsync(IDbConnection connection, IDbTransaction? transaction = null)
        {
            var tables = ModuleDefinitions.All.Select(x => x.Table).ToArray();
            var existing = await connection.QueryAsync<string>(
                "SELECT t.Name FROM (SELECT value AS Name FROM STRING_SPLIT(@tables, '|')) t WHERE OBJECT_ID(t.Name) IS NOT NULL",
                new { tables = string.Join('|', tables) }, transaction);

            return existing.ToHashSet();
        }
    }
}
