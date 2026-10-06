using System.Data;

namespace Domain.Shared.Abstractions.Repositorys
{
    public interface IConnectionAccessor
    {
        /// <summary>
        /// Get IDbConnection for methods that need direct connection access (e.g. splitOn queries)
        /// </summary>
        IDbConnection GetIDbConnection();
    }
}
