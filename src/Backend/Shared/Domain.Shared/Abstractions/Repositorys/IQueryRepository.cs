namespace Domain.Shared.Abstractions.Repositorys
{
    public interface IQueryRepository
    {
        /// <summary>
        /// Get a single record from Database
        /// </summary>
        Task<TEntity?> GetRecord<TEntity>(string? sql);

        /// <summary>
        /// Get multiple records from Database
        /// </summary>
        Task<IEnumerable<TEntity?>> GetRecords<TEntity>(string? sql);
    }
}
