using Application.Shared.Abstractions.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infrastructure.Shared.Locks
{
    /// <summary>
    /// IJobLock com sp_getapplock no banco das integrações (ConnectionStrings:Connection), PLANO-HOST-WEBAPI.md §4.7: lock do tipo
    /// APPLICATION sobre um nome, que não encosta em tabela, página ou linha; espera zero (nunca entra num deadlock) e preso à sessão
    /// (sem transação aberta durante o job). Cada trava tem uma conexão só dela: se o processo cair, a conexão fecha e a trava é
    /// solta sozinha
    /// </summary>
    public class SqlServerJobLock : IJobLock
    {
        // o sp_getapplock aceita nomes de até 255 caracteres
        private const int MaxResourceLength = 255;

        private readonly string? _connectionString;

        public SqlServerJobLock(IConfiguration configuration) =>
            _connectionString = configuration.GetConnectionString("Connection");

        public IDisposable? TryAcquire(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length > MaxResourceLength)
                throw new ArgumentException($"A chave da trava precisa ter de 1 a {MaxResourceLength} caracteres.", nameof(key));

            var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "sp_getapplock";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Resource", key);
                command.Parameters.AddWithValue("@LockMode", "Exclusive");
                command.Parameters.AddWithValue("@LockOwner", "Session");
                command.Parameters.AddWithValue("@LockTimeout", 0);
                command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue });
                command.ExecuteNonQuery();

                // 0 e 1 = trava concedida; -1 = ocupada (timeout zero); -2, -3 e -999 = cancelada, deadlock ou erro
                if ((int)command.Parameters["@Result"].Value >= 0)
                    return new Handle(connection, key);

                connection.Dispose();
                return null;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        private sealed class Handle : IDisposable
        {
            private readonly SqlConnection _connection;
            private readonly string _key;
            private bool _disposed;

            public Handle(SqlConnection connection, string key) =>
                (_connection, _key) = (connection, key);

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;

                try
                {
                    // solta antes de devolver a conexão ao pool; se falhar, fechar a conexão solta a trava do mesmo jeito
                    using var command = _connection.CreateCommand();
                    command.CommandText = "sp_releaseapplock";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Resource", _key);
                    command.Parameters.AddWithValue("@LockOwner", "Session");
                    command.ExecuteNonQuery();
                }
                finally
                {
                    _connection.Dispose();
                }
            }
        }
    }
}
