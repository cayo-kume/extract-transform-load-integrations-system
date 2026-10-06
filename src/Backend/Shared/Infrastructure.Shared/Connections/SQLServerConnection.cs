using Infrastructure.Shared.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.Shared.Connections
{
    public class SQLServerConnection : ISQLServerConnection, IDisposable
    {
        private readonly string? _connectionstring;
        private readonly string? _auditingConnectionstring;
        private IDbConnection _connection;
        private SqlConnection _conn;

        public SQLServerConnection(IConfiguration configuration)
        {
            _connectionstring = configuration.GetConnectionString("Connection");
            _auditingConnectionstring = configuration.GetConnectionString("AuditingConnection")
                ?? _connectionstring?.Replace("NEWBLOOMERS", "AUDITING");
        }

        public void Dispose() => _connection?.Dispose();

        public SqlConnection GetDbConnection()
        {
            try
            {
                _conn = new SqlConnection(_connectionstring);
                _conn.Open();
                return _conn;
            }
            catch (Exception ex)
            {
                _conn?.Dispose();
                throw new InvalidOperationException("Failed to establish a database connection.", ex);
            }
        }

        public IDbConnection GetIDbConnection()
        {
            try
            {
                _connection = new SqlConnection(_connectionstring);
                _connection.Open();
                return _connection;
            }
            catch (Exception ex)
            {
                _connection?.Dispose();
                throw new InvalidOperationException("Failed to establish a database connection.", ex);
            }
        }

        public SqlConnection GetDbAuditingConnection()
        {
            try
            {
                _conn = new SqlConnection(_auditingConnectionstring);
                _conn.Open();
                return _conn;
            }
            catch (Exception ex)
            {
                _conn?.Dispose();
                throw new InvalidOperationException("Failed to establish a database connection.", ex);
            }
        }

        public IDbConnection GetIDbAuditingConnection()
        {
            try
            {
                _connection = new SqlConnection(_auditingConnectionstring);
                _connection.Open();
                return _connection;
            }
            catch (Exception ex)
            {
                _connection?.Dispose();
                throw new InvalidOperationException("Failed to establish a database connection.", ex);
            }
        }
    }
}
