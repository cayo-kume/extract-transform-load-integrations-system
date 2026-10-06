using Infrastructure.Shared.Abstractions;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Infrastructure.Shared.Connections
{
    public class PostgreSQLConnection : IPostgreSQLConnection, IDisposable
    {
        private readonly string? _connectionstring;
        private IDbConnection _connection;

        public PostgreSQLConnection(IDbConnection connection) =>
            _connection = connection;

        public PostgreSQLConnection(IConfiguration configuration) =>
            _connectionstring = configuration.GetConnectionString("PostgreSQLConnection");

        public void Dispose() => _connection?.Dispose();

        public IDbConnection GetIDbConnection()
        {
            try
            {
                _connection = new NpgsqlConnection(_connectionstring);
                _connection.Open();
                return _connection;
            }
            catch (Exception ex)
            {
                _connection?.Dispose();
                throw new InvalidOperationException("Failed to establish a PostgreSQL database connection.", ex);
            }
        }
    }
}
