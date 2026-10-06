using System.Data;

namespace Infrastructure.Shared.Abstractions
{
    public interface IMySQLConnection
    {
        public IDbConnection GetIDbConnection();
    }
}
