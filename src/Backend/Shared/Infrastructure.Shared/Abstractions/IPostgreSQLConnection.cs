using System.Data;

namespace Infrastructure.Shared.Abstractions
{
    public interface IPostgreSQLConnection
    {
        public IDbConnection GetIDbConnection();
    }
}
