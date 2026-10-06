using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.Shared.Abstractions
{
    public interface ISQLServerConnection
    {
        public IDbConnection GetIDbConnection();
        public IDbConnection GetIDbAuditingConnection();
        public SqlConnection GetDbConnection();
        public SqlConnection GetDbAuditingConnection();
    }
}
