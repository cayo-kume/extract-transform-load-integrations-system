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
        private readonly ISharedRepository _coreRepository;

        // as cargas completas (fatos, agregados) podem passar dos 45 min do StoredProcedureTimeoutSeconds do Shared na carga inicial
        private const int ProcedureTimeoutSeconds = 7200;

        // nome de objeto do DW: [schema].[nome] ou schema.nome, só letras, números e sublinhado (os nomes vêm do catálogo e do service)
        private static readonly Regex ObjectName = new(@"^\[?[A-Za-z_][A-Za-z0-9_]*\]?\.\[?[A-Za-z_][A-Za-z0-9_]*\]?$", RegexOptions.Compiled);

        public DataWarehouseRepository(ISharedRepository coreRepository) =>
            _coreRepository = coreRepository;

        public async Task<IEnumerable<LandingSource>> GetLandingSources()
        {
            const string sql = "select source_schema, source_table, detect_missing, history_frequency from [dw_control].LandingSource order by source_schema, source_table";

            var sources = await _coreRepository.GetRecords<LandingSource>(sql);

            return sources.Where(x => x is not null).Select(x => x!);
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

            return await _coreRepository.GetRecord<int>($"select count(*) from {viewName}");
        }

        public async Task<bool> InsertCheckResult(CheckResult checkResult)
        {
            const string sql = "insert into [dw_control].CheckResult (check_name, executed_at, error_rows, passed) values (@check_name, @executed_at, @error_rows, @passed)";

            return await _coreRepository.ExecuteCommand(sql, checkResult);
        }
    }
}
