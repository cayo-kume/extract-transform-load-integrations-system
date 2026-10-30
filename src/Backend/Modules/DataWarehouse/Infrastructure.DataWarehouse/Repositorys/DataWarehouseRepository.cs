using Application.DataWarehouse.Abstractions.Handlers.Commands;
using Dapper;
using Domain.DataWarehouse.Abstractions.Repositorys;
using Domain.DataWarehouse.Entities.Control;
using Domain.Shared.Abstractions.Repositorys;
using System.Data;
using System.Text.RegularExpressions;

namespace Infrastructure.DataWarehouse.Repositorys
{
    public class DataWarehouseRepository : IDataWarehouseRepository
    {
        private readonly IDataWarehouseCommandHandler _dataWarehouseCommandHandler;
        private readonly ISharedRepository _coreRepository;

        // as cargas completas (fatos, agregados) podem passar dos 45 min do StoredProcedureTimeoutSeconds do Shared na carga inicial
        private const int ProcedureTimeoutSeconds = 7200;

        // nome de objeto do DW: [schema].[nome] ou schema.nome, só letras, números e sublinhado (os nomes vêm do catálogo e do service)
        private static readonly Regex ObjectName = new(@"^\[?[A-Za-z_][A-Za-z0-9_]*\]?\.\[?[A-Za-z_][A-Za-z0-9_]*\]?$", RegexOptions.Compiled);

        public DataWarehouseRepository(IDataWarehouseCommandHandler dataWarehouseCommandHandler, ISharedRepository coreRepository) =>
            (_dataWarehouseCommandHandler, _coreRepository) = (dataWarehouseCommandHandler, coreRepository);

        public async Task<IEnumerable<LandingSource>> GetLandingSources()
        {
            string sql = _dataWarehouseCommandHandler.CreateGetLandingSourcesQuery();

            var sources = await _coreRepository.GetRecords<LandingSource>(sql);

            return sources.Where(x => x is not null).Select(x => x!);
        }

        public async Task<IEnumerable<LandingSource>> GetLandingSourcesBySchema(string sourceSchema)
        {
            string sql = _dataWarehouseCommandHandler.CreateGetLandingSourcesBySchemaQuery();

            // o GetRecords do Shared não recebe parâmetros
            using var conn = _coreRepository.GetIDbConnection();

            return await conn.QueryAsync<LandingSource>(sql, new { sourceSchema });
        }

        public async Task<int> ExecuteProcedure(string procedureName, IDictionary<string, object?>? parameters = null)
        {
            if (!ObjectName.IsMatch(procedureName))
                throw new ArgumentException($"Nome de procedure inválido: {procedureName}", nameof(procedureName));

            var dynamicParameters = new DynamicParameters();

            foreach (var parameter in parameters ?? new Dictionary<string, object?>())
                dynamicParameters.Add(parameter.Key, parameter.Value);

            dynamicParameters.Add("rows", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using var conn = _coreRepository.GetIDbConnection();

            await conn.ExecuteAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure, commandTimeout: ProcedureTimeoutSeconds);

            return dynamicParameters.Get<int?>("rows") ?? 0;
        }

        public async Task<int> CountRows(string viewName)
        {
            if (!ObjectName.IsMatch(viewName))
                throw new ArgumentException($"Nome de view inválido: {viewName}", nameof(viewName));

            return await _coreRepository.GetRecord<int>(_dataWarehouseCommandHandler.CreateCountRowsQuery(viewName));
        }

        public async Task<bool> InsertCheckResult(CheckResult checkResult)
        {
            string sql = _dataWarehouseCommandHandler.CreateInsertCheckResultQuery();

            return await _coreRepository.ExecuteCommand(sql, checkResult);
        }
    }
}
