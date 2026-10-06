using Infrastructure.Shared.Abstractions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace Infrastructure.Shared.Connections
{
    public class MySQLConnection : IMySQLConnection, IDisposable
    {
        private readonly string? _connectionstring;
        private IDbConnection _connection;

        public MySQLConnection(IDbConnection connection) =>
            _connection = connection;

        public MySQLConnection(IConfiguration configuration) =>
            _connectionstring = configuration.GetConnectionString("MySQLConnection");

        public void Dispose() => _connection?.Dispose();

        public IDbConnection GetIDbConnection()
        {
            try
            {
                _connection = new MySqlConnection(_connectionstring);
                _connection.Open();
                return _connection;
            }
            catch (Exception ex)
            {
                _connection?.Dispose();
                throw new InvalidOperationException("Failed to establish a MySQL database connection.", ex);
            }
        }
    }
}
