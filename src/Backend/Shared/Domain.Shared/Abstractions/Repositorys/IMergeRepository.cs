namespace Domain.Shared.Abstractions.Repositorys
{
    public interface IMergeRepository
    {
        /// <summary>
        /// Call synchronization stored procedure from Database
        /// </summary>
        Task<bool> CallDbProcMerge(string schemaName, string tableName, Guid? parentExecutionGUID);

        /// <summary>
        /// Call synchronization stored procedure from Database
        /// </summary>
        Task<bool> CallDbProcMerge(string schemaName, string tableName);
    }
}
