using System.Data;

namespace Domain.Shared.Abstractions.Repositorys
{
    public interface ICommandRepository
    {
        /// <summary>
        /// Execute simple sql command in Database
        /// </summary>
        Task<bool> ExecuteCommand(string? sql);

        /// <summary>
        /// Execute sql command with entity parameters in Database
        /// </summary>
        Task<bool> ExecuteCommand(string? sql, object entity);

        /// <summary>
        /// Execute sql command with entity parameters in Auditing Database
        /// </summary>
        Task<bool> ExecuteAuditingCommand(string? sql, object entity);

        /// <summary>
        /// Execute sql command with dictionary parameters in Database
        /// </summary>
        Task<bool> ExecuteCommand(string? sql, IDictionary<string, object?> parameters);

        /// <summary>
        /// Execute sql command with dictionary parameters and custom CommandType in Database
        /// </summary>
        Task<bool> ExecuteCommand(string? sql, IDictionary<string, object?> parameters, CommandType commandType);

        /// <summary>
        /// Insert a single record into Database
        /// </summary>
        Task<bool> InsertRecord(string? sql, object entity);
    }
}
