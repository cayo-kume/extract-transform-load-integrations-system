using System.Data;

namespace Domain.Shared.Abstractions.Repositorys
{
    public interface IBulkInsertRepository
    {
        /// <summary>
        /// Create a System DATA .NET DataTable from entity properties
        /// </summary>
        DataTable CreateSystemDataTable<TEntity>(string? tableName, TEntity entity);

        /// <summary>
        /// Create a System DATA .NET DataTable with custom column types
        /// </summary>
        DataTable CreateSystemDataTable<TEntity>(string tableName, string[] columnNames, Type[] columnTypes, TEntity entity);

        /// <summary>
        /// Fill System DataTable with data from a list of entities
        /// </summary>
        void FillSystemDataTable<TEntity>(DataTable dataTable, List<TEntity> data);

        /// <summary>
        /// Execute bulk insert on default schema from Database.
        /// Default schema is configured via "BulkInsert:DefaultSchema" in appsettings.
        /// </summary>
        [Obsolete("Use the overload with explicit schema parameter: BulkInsertIntoTableRaw(DataTable, string schema)")]
        bool BulkInsertIntoTableRaw(DataTable dataTable);

        /// <summary>
        /// Execute bulk insert on specific schema from Database
        /// </summary>
        bool BulkInsertIntoTableRaw(DataTable dataTable, string schema);

        /// <summary>
        /// Execute bulk insert on specific schema from Auditing Database
        /// </summary>
        bool BulkInsertIntoAuditingTableRaw(DataTable dataTable, string schema);
    }
}
