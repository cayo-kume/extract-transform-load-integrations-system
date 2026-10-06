using Dapper;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Shared.Extensions;
using Infrastructure.Shared.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace Infrastructure.Shared.Repositorys
{
    public class SharedRepository : ISharedRepository
    {
        private const int BulkCopyTimeoutSeconds = 360;
        private const int QueryTimeoutSeconds = 360;
        private const int StoredProcedureTimeoutSeconds = 2700;
        private const int CommandTimeoutSeconds = 3600;

        private readonly ISQLServerConnection _sqlServerConnection;
        private readonly string _defaultBulkInsertSchema;

        public SharedRepository(ISQLServerConnection sqlServerConnection, IConfiguration configuration)
        {
            _sqlServerConnection = sqlServerConnection;
            _defaultBulkInsertSchema = configuration["BulkInsert:DefaultSchema"] ?? "untreated";
        }

        [Obsolete("Use the overload with explicit schema parameter: BulkInsertIntoTableRaw(DataTable, string schema)")]
        public bool BulkInsertIntoTableRaw(DataTable dataTable) =>
            BulkInsertIntoTableRaw(dataTable, _defaultBulkInsertSchema);

        public bool BulkInsertIntoTableRaw(DataTable dataTable, string schema)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetDbConnection())
                {
                    using var bulkCopy = new SqlBulkCopy(conn);
                    bulkCopy.DestinationTableName = $"[{schema}].[{dataTable.TableName}]";
                    bulkCopy.BatchSize = dataTable.Rows.Count;
                    bulkCopy.BulkCopyTimeout = BulkCopyTimeoutSeconds;
                    foreach (DataColumn c in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                    }
                    bulkCopy.WriteToServer(dataTable);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to bulk insert records on database",
                    innerException: ex
                );
            }
        }

        public bool BulkInsertIntoAuditingTableRaw(DataTable dataTable, string schema)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetDbAuditingConnection())
                {
                    using var bulkCopy = new SqlBulkCopy(conn);
                    bulkCopy.DestinationTableName = $"[{schema}].[{dataTable.TableName}]";
                    bulkCopy.BatchSize = dataTable.Rows.Count;
                    bulkCopy.BulkCopyTimeout = BulkCopyTimeoutSeconds;
                    foreach (DataColumn c in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                    }
                    bulkCopy.WriteToServer(dataTable);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to bulk insert records on database",
                    innerException: ex
                );
            }
        }

        public DataTable CreateSystemDataTable<TEntity>(string? tableName, TEntity entity)
        {
            try
            {
                var properties = entity.GetType().GetFilteredProperties();
                var dataTable = new DataTable(tableName);

                foreach (PropertyInfo prop in properties)
                {
                    dataTable.Columns.Add(prop.GetColumnName(), Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error trying to create system datatable",
                    innerException: ex
                );
            }
        }

        public DataTable CreateSystemDataTable<TEntity>(string tableName, string[] columnNames, Type[] columnTypes, TEntity entity)
        {
            var properties = entity.GetType().GetFilteredProperties();
            var dataTable = new DataTable(tableName);

            for (int i = 0; i < properties.Count(); i++)
            {
                for (int j = 0; j < columnNames.Length; j++)
                {
                    var colName = properties[i].GetColumnName();

                    if (colName == columnNames[j])
                        dataTable.Columns.Add(colName, columnTypes[j]);

                    else if (!dataTable.Columns.Contains(colName) && !columnNames.Contains(colName))
                        dataTable.Columns.Add(colName, Nullable.GetUnderlyingType(properties[i].PropertyType) ?? properties[i].PropertyType);
                }
            }

            return dataTable;
        }

        public void FillSystemDataTable<TEntity>(DataTable dataTable, List<TEntity> data)
        {
            try
            {
                if (data.Count() > 0)
                {
                    var properties = data.FirstOrDefault().GetType().GetFilteredProperties();

                    foreach (var item in data)
                    {
                        var rowValues = new object[properties.Length];

                        for (int i = 0; i < properties.Length; i++)
                        {
                            var prop = properties[i];
                            var value = prop.GetValue(item);

                            rowValues[i] = value ?? DBNull.Value;
                        }

                        dataTable.Rows.Add(rowValues);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to fill the system datatable",
                    innerException: ex
                );
            }
        }

        public async Task<bool> CallDbProcMerge(string schemaName, string tableName, Guid? parentExecutionGUID)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Guid", parentExecutionGUID);

                    var result = await conn.ExecuteAsync($"[{schemaName}].[sp_{tableName}_Sync]", param: parameters, commandType: CommandType.StoredProcedure, commandTimeout: StoredProcedureTimeoutSeconds);

                    if (result > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to run sync proc on database",
                    innerException: ex
                );
            }
        }

        public async Task<bool> CallDbProcMerge(string schemaName, string tableName)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var result = await conn.ExecuteAsync($"[{schemaName}].[sp_{tableName}_Sync]", commandType: CommandType.StoredProcedure, commandTimeout: StoredProcedureTimeoutSeconds);

                    if (result > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to run sync proc on database",
                    innerException: ex
                );
            }
        }

        public async Task<TEntity?> GetRecord<TEntity>(string? sql)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    return await conn.QueryFirstOrDefaultAsync<TEntity?>(sql: sql, commandTimeout: QueryTimeoutSeconds);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to get record from database | {sql}",
                    innerException: ex
                );
            }
        }

        public async Task<IEnumerable<TEntity?>> GetRecords<TEntity>(string? sql)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetDbConnection())
                {
                    var result = await conn.QueryAsync<TEntity>(sql: sql, commandTimeout: QueryTimeoutSeconds);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to get records from database | {sql}",
                    innerException: ex
                );
            }
        }

        public async Task<bool> InsertRecord(string? sql, object entity)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var result = await conn.ExecuteAsync(sql: sql, param: entity, commandTimeout: QueryTimeoutSeconds);

                    if (result > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to insert record on database | {sql}",
                    innerException: ex
                );
            }
        }

        public async Task<bool> ExecuteCommand(string? sql)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var result = await conn.ExecuteAsync(sql: sql, commandTimeout: CommandTimeoutSeconds);

                    if (result > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to execute sql command on database | {sql}",
                    innerException: ex
                );
            }
        }

        public async Task<bool> ExecuteCommand(string? sql, object entity)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var result = await conn.ExecuteAsync(sql: sql, entity, commandTimeout: CommandTimeoutSeconds);

                    if (result > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to execute sql command on database | {sql}",
                    innerException: ex
                );
            }
        }

        public async Task<bool> ExecuteAuditingCommand(string? sql, object entity)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetIDbAuditingConnection())
                {
                    var result = await conn.ExecuteAsync(sql: sql, entity, commandTimeout: CommandTimeoutSeconds);

                    if (result > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to execute sql command on database | {sql}",
                    innerException: ex
                );
            }
        }

        public async Task<bool> ExecuteCommand(string? sql, IDictionary<string, object?> parameters)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var dynamicParams = new DynamicParameters();
                    foreach (var kvp in parameters)
                        dynamicParams.Add(kvp.Key, kvp.Value);

                    var result = await conn.ExecuteAsync(sql: sql, param: dynamicParams, commandTimeout: CommandTimeoutSeconds);

                    if (result > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to execute sql command on database | {sql}",
                    innerException: ex
                );
            }
        }

        public async Task<bool> ExecuteCommand(string? sql, IDictionary<string, object?> parameters, CommandType commandType)
        {
            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var dynamicParams = new DynamicParameters();
                    foreach (var kvp in parameters)
                        dynamicParams.Add(kvp.Key, kvp.Value);

                    var result = await conn.ExecuteAsync(sql: sql, param: dynamicParams, commandType: commandType, commandTimeout: CommandTimeoutSeconds);

                    if (result > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    message: $"Error when trying to execute sql command on database | {sql}",
                    innerException: ex
                );
            }
        }

        public IDbConnection GetIDbConnection()
        {
            return _sqlServerConnection.GetIDbConnection();
        }
    }
}
